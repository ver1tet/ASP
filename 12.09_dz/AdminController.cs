using ASP_P42.Data;
using ASP_P42.Data.Entities;
using ASP_P42.Models.Admin;
using ASP_P42.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace ASP_P42.Controllers
{
    // Д.З. 12.09.2026 - Реалізувати валідацію моделі форми додавання нового товару
    public class AdminController(IStorageService storageService, DataAccessor dataAccessor) : Controller
    {
        private readonly IStorageService _storageService = storageService;
        private readonly DataAccessor _dataAccessor = dataAccessor;


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Product()
        {
            AdminGroupViewModel viewModel = new()
            {
                Groups = _dataAccessor.GetAllProductGroups(isIncludeHidden: true),
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(AdminAddProductFormModel formModel)
        {
            // Д.З. 12.09.2026: Валідація через Data Annotations атрибути моделі
            if (!ModelState.IsValid)
            {
                // Збираємо всі повідомлення про помилки в один рядок
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return BadRequest(string.Join("; ", errors));
            }

            try
            {
                await _dataAccessor.IsProductFormModelValidAsync(formModel);

                String? imageUrl = null;
                if (formModel.Image != null)
                {
                    imageUrl = _storageService.Save(formModel.Image);
                }

                await _dataAccessor.AddNewProduct(formModel, imageUrl);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult Group()
        {
            AdminGroupViewModel viewModel = new()
            {
                Groups = _dataAccessor.GetAllProductGroups(isIncludeHidden: true),
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddGroup(AdminAddGroupFormModel formModel)
        {
            // Д.З. 10.09.2026: Валідація через Data Annotations атрибути моделі
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return BadRequest(string.Join("; ", errors));
            }

            try
            {
                if (formModel.Id.HasValue)
                {
                    await _dataAccessor.UpdateProductGroup(new()
                    {
                        Id = formModel.Id.Value,
                        ParentId = formModel.ParentId,
                        Name = formModel.Name,
                        Description = formModel.Description,
                        Slug = formModel.Slug,
                        IsHidden = formModel.IsHidden,
                        ImageUrl = formModel.Image != null ? "/storage/image/" + _storageService.Save(formModel.Image) : null!,
                        OrderInPrice = formModel.Order,
                    });
                    return Ok(formModel.Id.Value);
                }
                else
                {
                    Guid newGroupId = await _dataAccessor.AddNewProductGroup(new()
                    {
                        ParentId = formModel.ParentId,
                        Name = formModel.Name,
                        Description = formModel.Description,
                        Slug = formModel.Slug,
                        IsHidden = formModel.IsHidden,
                        ImageUrl = "/storage/image/" + _storageService.Save(formModel.Image),
                        OrderInPrice = formModel.Order,
                    });
                    return Ok(newGroupId);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteGroup(Guid id)
        {
            try
            {
                await _dataAccessor.DeleteProductGroup(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
