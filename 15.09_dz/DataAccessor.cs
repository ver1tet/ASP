using ASP_P42.Data.Entities;
using ASP_P42.Models.Admin;
using Microsoft.EntityFrameworkCore;

namespace ASP_P42.Data
{
    public class DataAccessor(DataContext dataContext)
    {
        private readonly DataContext _dataContext = dataContext;

        /* Д.З. 15.09.2026 - Реалізувати метод отримання унікального ідентифікатора
         * з бази даних. SQL Server має функцію NEWID(), яка генерує
         * унікальний ідентифікатор.
         *
         * Використовуємо FromSqlRaw для виконання скалярного SQL-запиту.
         * SQL Server: SELECT NEWID() повертає новий GUID безпосередньо з БД,
         * що гарантує унікальність на рівні сервера бази даних.
         */
        public Guid GetDbIdentity()
        {
            // Виконуємо SQL-запит до SQL Server, який генерує GUID на стороні БД
            var result = _dataContext.Database
                .SqlQueryRaw<Guid>("SELECT NEWID() AS Value")
                .AsEnumerable()
                .First();
            return result;
        }

        public List<Entities.ProductGroup> GetAllProductGroups(bool isIncludeHidden = false)
        {
            IQueryable<Entities.ProductGroup> query = _dataContext.ProductGroups;
            if (!isIncludeHidden)
            {
                query = query.Where(g => g.IsHidden == 0);
            }
            return [..query.OrderBy(g => g.OrderInPrice)];
        }

        public async Task<bool> IsGroupSlugUniqueAsync(String slug, Guid? excludeId = null)
        {
            return !_dataContext.ProductGroups
                .Any(g => g.Slug == slug && g.Id != excludeId);
        }

        public async Task<Guid> AddNewProductGroup(Entities.ProductGroup productGroup)
        {
            if (!await IsGroupSlugUniqueAsync(productGroup.Slug))
            {
                throw new Exception($"Slug '{productGroup.Slug}' вже використовується іншою групою");
            }

            Guid id = GetDbIdentity();
            productGroup.Id = id;
            _dataContext.ProductGroups.Add(productGroup);
            await _dataContext.SaveChangesAsync();
            return id;
        }

        public async Task<Guid> AddNewProduct(AdminAddProductFormModel formModel, String? imageUrl)
        {
            Guid id = GetDbIdentity();
            if (formModel.ProductId != null)
            {
                _dataContext.ProductVersions.Add(new()
                {
                    Id = id,
                    ProductId = (await GetProductById(formModel.ProductId.Value))!.Id,
                    ImageUrl = imageUrl,
                    Price = (decimal)formModel.Price,
                    Stock = formModel.Stock,
                    OrderInPrice = formModel.Order,
                    Slug = formModel.Slug,
                    IsHidden = formModel.IsHidden,
                    Version = formModel.Name
                });
            }
            else
            {
                Entities.ProductGroup group = await GetProductGroupById(formModel.GroupId)!;
                Guid productId = GetDbIdentity();
                _dataContext.Products.Add(new()
                {
                    Id = productId,
                    GroupId = group.Id,
                    Name = formModel.Name,
                    Description = formModel.Description,
                    ImageUrl = imageUrl,
                    IsHidden = formModel.IsHidden,
                    OrderInPrice = formModel.Order,
                    Slug = formModel.Slug,
                });
                _dataContext.ProductVersions.Add(new()
                {
                    Id = id,
                    ProductId = productId,
                    ImageUrl = imageUrl,
                    Price = (decimal)formModel.Price,
                    Stock = formModel.Stock,
                    OrderInPrice = 1,
                    Slug = formModel.Slug,
                    IsHidden = formModel.IsHidden,
                });
            }

            await _dataContext.SaveChangesAsync();
            return id;
        }

        public async Task<bool> IsProductFormModelValidAsync(AdminAddProductFormModel formModel)
        {
            var group = await GetProductGroupById(formModel.GroupId)
                   ?? throw new Exception($"Product group not found with id='{formModel.GroupId}'");
            if (formModel.ProductId != null)
            {
                _ = await GetProductById(formModel.ProductId.Value)
                ?? throw new Exception($"Product not found with id='{formModel.ProductId}'");
            }
            if (formModel.Slug != null)
            {
                if (_dataContext.Products.Any(p => p.Slug == formModel.Slug))
                {
                    throw new Exception($"Slug '{formModel.Slug}' is already in use by other product");
                }
            }
            return true;
        }

        public async Task<Entities.ProductGroup?> GetProductGroupById(Guid guid)
        {
            return _dataContext.ProductGroups.FirstOrDefault(g => g.Id == guid);
        }

        public async Task<Entities.Product?> GetProductById(Guid guid)
        {
            return _dataContext.Products.FirstOrDefault(p => p.Id == guid);
        }

        public async Task UpdateProductGroup(Entities.ProductGroup productGroup)
        {
            var existing = await GetProductGroupById(productGroup.Id)
                ?? throw new Exception("Group not found");

            if (!await IsGroupSlugUniqueAsync(productGroup.Slug, productGroup.Id))
            {
                throw new Exception($"Slug '{productGroup.Slug}' вже використовується іншою групою");
            }

            existing.Name = productGroup.Name;
            existing.Description = productGroup.Description;
            existing.Slug = productGroup.Slug;
            existing.IsHidden = productGroup.IsHidden;
            existing.OrderInPrice = productGroup.OrderInPrice;
            existing.ParentId = productGroup.ParentId;
            if (productGroup.ImageUrl != null)
            {
                existing.ImageUrl = productGroup.ImageUrl;
            }

            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteProductGroup(Guid id)
        {
            var existing = await GetProductGroupById(id)
                ?? throw new Exception("Group not found");

            existing.IsHidden = 1;

            await _dataContext.SaveChangesAsync();
        }
    }
}
