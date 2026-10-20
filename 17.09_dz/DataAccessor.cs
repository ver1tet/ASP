using ASP_P42.Data.Entities;
using ASP_P42.Models.Admin;
using Microsoft.EntityFrameworkCore;

namespace ASP_P42.Data
{
    
    public class DataAccessor(DataContext dataContext)
    {
        private readonly DataContext _dataContext = dataContext;

        public Guid GetDbIdentity()
        {
            var result = _dataContext.Database
                .SqlQueryRaw<Guid>("SELECT NEWID() AS Value")
                .AsEnumerable()
                .First();
            return result;
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


        public List<Entities.ProductGroup> GetAllProductGroups(bool isIncludeHidden = false)
        {
            IQueryable<Entities.ProductGroup> query = _dataContext.ProductGroups;
            if (!isIncludeHidden)
            {
                query = query.Where(g => g.IsHidden == 0);
            }
            return [..query.OrderBy(g => g.OrderInPrice)];
        }


        public async Task<Entities.ProductGroup?> GetProductGroupById(Guid id)
        {
            return _dataContext.ProductGroups
                .Include(g => g.Children)
                .Include(g => g.Products)
                .FirstOrDefault(g => g.Id == id);
        }

  
        public async Task<Entities.ProductGroup?> GetProductGroupBySlug(String slug)
        {
            return _dataContext.ProductGroups
                .Include(g => g.Children)
                .Include(g => g.Products)
                .FirstOrDefault(g => g.Slug == slug);
        }


        public async Task UpdateProductGroup(Entities.ProductGroup productGroup)
        {
            var existing = await GetProductGroupById(productGroup.Id)
                ?? throw new Exception($"Групу з Id='{productGroup.Id}' не знайдено");

            if (!await IsGroupSlugUniqueAsync(productGroup.Slug, productGroup.Id))
            {
                throw new Exception($"Slug '{productGroup.Slug}' вже використовується іншою групою");
            }

            existing.Name         = productGroup.Name;
            existing.Description  = productGroup.Description;
            existing.Slug         = productGroup.Slug;
            existing.IsHidden     = productGroup.IsHidden;
            existing.OrderInPrice = productGroup.OrderInPrice;
            existing.ParentId     = productGroup.ParentId;
            if (productGroup.ImageUrl != null)
            {
                existing.ImageUrl = productGroup.ImageUrl;
            }

            await _dataContext.SaveChangesAsync();
        }


        public async Task DeleteProductGroup(Guid id)
        {
            var existing = await GetProductGroupById(id)
                ?? throw new Exception($"Групу з Id='{id}' не знайдено");

            existing.IsHidden = 1;
            await _dataContext.SaveChangesAsync();
        }


        public async Task HardDeleteProductGroup(Guid id)
        {
            var existing = await GetProductGroupById(id)
                ?? throw new Exception($"Групу з Id='{id}' не знайдено");

            _dataContext.ProductGroups.Remove(existing);
            await _dataContext.SaveChangesAsync();
        }

        // Product

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

        public async Task<Entities.Product?> GetProductById(Guid guid)
        {
            return _dataContext.Products.FirstOrDefault(p => p.Id == guid);
        }
    }
}
