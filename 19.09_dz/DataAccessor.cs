using ASP_P42.Data.Entities;
using ASP_P42.Models.Admin;
using ASP_P42.Models.User;
using ASP_P42.Services.Kdf;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ASP_P42.Data
{
    /* Д.З. 19.09.2026 - Accessor: перенести логіку реєстрації та автентифікації
     * користувача з UserController у DataAccessor.
     *
     * Методи перенесено:
     *   RegisterUser()      — реєстрація нового користувача
     *   AuthenticateUser()  — перевірка логіна і пароля, повертає UserAccess або null
     */
    public class DataAccessor(DataContext dataContext, IKdfService kdfService)
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IKdfService _kdfService  = kdfService;

        public Guid GetDbIdentity()
        {
            var result = _dataContext.Database
                .SqlQueryRaw<Guid>("SELECT NEWID() AS Value")
                .AsEnumerable()
                .First();
            return result;
        }

        // ===================== User: Register & Authenticate =====================

        /// <summary>
        /// Реєстрація нового користувача.
        /// Виконує всю валідацію та запис у БД.
        /// Кидає Exception з повідомленням про помилку при невалідних даних.
        /// </summary>
        public void RegisterUser(UserSignupFormModel formModel)
        {
            // --- Дешеві перевірки (без БД) ---
            if (formModel == null)
                throw new ArgumentNullException(nameof(formModel), "Data structure non-bindable to model");

            if (!formModel.IsAgree)
                throw new Exception("You should confirm site policy (agreement)");

            const String requiredMessage = " could not be empty";

            if (String.IsNullOrEmpty(formModel.Login))
                throw new Exception(nameof(formModel.Login) + requiredMessage);

            if (String.IsNullOrEmpty(formModel.FullName))
                throw new Exception(nameof(formModel.FullName) + requiredMessage);

            if (String.IsNullOrEmpty(formModel.Email))
                throw new Exception(nameof(formModel.Email) + requiredMessage);

            if (String.IsNullOrEmpty(formModel.Password))
                throw new Exception(nameof(formModel.Password) + requiredMessage);

            if (formModel.Password != formModel.Repeat)
                throw new Exception("Password and Repeat mismatch");

            // --- Формат та нормалізація ---
            formModel.FullName = formModel.FullName.Trim();
            if (formModel.FullName.Length < 2)
                throw new Exception(nameof(formModel.FullName) + " too short (2 symbols at least)");

            formModel.Login = formModel.Login.Trim();
            if (formModel.Login.Length < 2)
                throw new Exception(nameof(formModel.Login) + " too short (2 symbols at least)");

            if (formModel.Login.Contains(':'))
                throw new Exception(nameof(formModel.Login) + " could not contain colon (':')");

            formModel.Email = formModel.Email.Trim();
            if (!Regex.IsMatch(formModel.Email,
                @"^\w+([-+.']\w+)*@\w+([-.]\\w+)*\.\w+([-.]\\w+)*$"))
                throw new Exception(nameof(formModel.Email) + " has invalid format");

            // --- Перевірка БД: унікальність логіна ---
            if (_dataContext.UserAccesses.Any(ua => ua.Login == formModel.Login))
                throw new Exception(nameof(formModel.Login) + $" '{formModel.Login}' is already in use");

            // --- Збереження ---
            Guid userId = GetDbIdentity();
            _dataContext.UsersData.Add(new()
            {
                Id = userId,
                FullName = formModel.FullName,
                Email = formModel.Email,
                Phone = formModel.Phone,
                RegisteredAt = DateTime.Now,
                Birthdate = default,
            });

            String salt = Guid.NewGuid().ToString();
            _dataContext.UserAccesses.Add(new()
            {
                Id = GetDbIdentity(),
                UserId = userId,
                RoleId = _dataContext.UserRoles.First(r => r.Name == "User").Id,
                Login = formModel.Login,
                Salt = salt,
                Dk = _kdfService.Dk(formModel.Password, salt),
            });

            _dataContext.SaveChanges();
        }

        /// <summary>
        /// Автентифікація за логіном та паролем (Basic Auth).
        /// Повертає UserAccess якщо облікові дані коректні, або null якщо пароль невірний.
        /// Кидає Exception якщо заголовок Authorization відсутній або невалідний.
        /// </summary>
        public UserAccess? AuthenticateBasic(String authHeader)
        {
            if (String.IsNullOrEmpty(authHeader))
                throw new Exception("Missing Authorization header");

            const String scheme = "Basic ";
            if (!authHeader.StartsWith(scheme))
                throw new Exception("Authorization scheme must be 'Basic'");

            String credentials = authHeader[scheme.Length..];
            byte[] rawData;
            try
            {
                rawData = Convert.FromBase64String(credentials);
            }
            catch
            {
                throw new Exception("Authorization credentials must be valid Base64::section 4");
            }

            String userPass;
            try
            {
                userPass = System.Text.Encoding.UTF8.GetString(rawData);
            }
            catch
            {
                throw new Exception("User-pass must be valid UTF8 string");
            }

            String[] parts = userPass.Split(':', 2);
            if (parts.Length != 2)
                throw new Exception("User-pass must be concatenated by ':'");

            String login    = parts[0];
            String password = parts[1];

            if (_dataContext.UserAccesses
                .Include(ua => ua.UserData)
                .Include(ua => ua.UserRole)
                .AsNoTracking()
                .FirstOrDefault(ua => ua.Login == login)
                is UserAccess userAccess)
            {
                String dk = _kdfService.Dk(password, userAccess.Salt);
                if (dk == userAccess.Dk)
                {
                    return userAccess;
                }
            }
            return null;
        }

        // ===================== ProductGroup CRUD =====================

        public async Task<bool> IsGroupSlugUniqueAsync(String slug, Guid? excludeId = null)
        {
            return !_dataContext.ProductGroups
                .Any(g => g.Slug == slug && g.Id != excludeId);
        }

        public List<Entities.ProductGroup> GetAllProductGroups(bool isIncludeHidden = false)
        {
            IQueryable<Entities.ProductGroup> query = _dataContext.ProductGroups;
            if (!isIncludeHidden)
                query = query.Where(g => g.IsHidden == 0);
            return [..query.OrderBy(g => g.OrderInPrice)];
        }

        public async Task<Guid> AddNewProductGroup(Entities.ProductGroup productGroup)
        {
            if (!await IsGroupSlugUniqueAsync(productGroup.Slug))
                throw new Exception($"Slug '{productGroup.Slug}' вже використовується іншою групою");

            Guid id = GetDbIdentity();
            productGroup.Id = id;
            _dataContext.ProductGroups.Add(productGroup);
            await _dataContext.SaveChangesAsync();
            return id;
        }

        public async Task<Entities.ProductGroup?> GetProductGroupById(Guid id)
        {
            return _dataContext.ProductGroups.FirstOrDefault(g => g.Id == id);
        }

        public async Task UpdateProductGroup(Entities.ProductGroup productGroup)
        {
            var existing = await GetProductGroupById(productGroup.Id)
                ?? throw new Exception("Group not found");

            if (!await IsGroupSlugUniqueAsync(productGroup.Slug, productGroup.Id))
                throw new Exception($"Slug '{productGroup.Slug}' вже використовується іншою групою");

            existing.Name         = productGroup.Name;
            existing.Description  = productGroup.Description;
            existing.Slug         = productGroup.Slug;
            existing.IsHidden     = productGroup.IsHidden;
            existing.OrderInPrice = productGroup.OrderInPrice;
            existing.ParentId     = productGroup.ParentId;
            if (productGroup.ImageUrl != null)
                existing.ImageUrl = productGroup.ImageUrl;

            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteProductGroup(Guid id)
        {
            var existing = await GetProductGroupById(id)
                ?? throw new Exception("Group not found");
            existing.IsHidden = 1;
            await _dataContext.SaveChangesAsync();
        }

        // ===================== Product =====================

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
            _ = await GetProductGroupById(formModel.GroupId)
                ?? throw new Exception($"Product group not found with id='{formModel.GroupId}'");
            if (formModel.ProductId != null)
                _ = await GetProductById(formModel.ProductId.Value)
                    ?? throw new Exception($"Product not found with id='{formModel.ProductId}'");
            if (formModel.Slug != null)
                if (_dataContext.Products.Any(p => p.Slug == formModel.Slug))
                    throw new Exception($"Slug '{formModel.Slug}' is already in use by other product");
            return true;
        }

        public async Task<Entities.Product?> GetProductById(Guid guid)
        {
            return _dataContext.Products.FirstOrDefault(p => p.Id == guid);
        }
    }
}
