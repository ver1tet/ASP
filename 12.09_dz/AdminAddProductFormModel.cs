using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ASP_P42.Models.Admin
{
    /* Д.З. 12.09.2026 - Реалізувати валідацію моделі форми
     * додавання нового товару
     * - назва (довжина, відсутність спецсимволів)
     * - опис - якщо передається, то перевіряється (довжина), інакше ігнорується
     * - Slug (унікальність, url-коректність) - якщо передається
     * - кількість - ціле позитивне число або "-1"
     * - ціна - позитивне число більше за 0.01
     */
    public class AdminAddProductFormModel
    {
        [FromForm(Name = "product-group")]
        [Required(ErrorMessage = "Група товару є обов'язковою")]
        public Guid GroupId { get; set; }

        [FromForm(Name = "product-id")]
        public Guid? ProductId { get; set; }   // якщо додається версія наявного товару

        // Назва: обов'язкова, від 2 до 200 символів, без спецсимволів
        [FromForm(Name = "product-name")]
        [Required(ErrorMessage = "Назва товару є обов'язковою")]
        [StringLength(200, MinimumLength = 2,
            ErrorMessage = "Назва повинна містити від 2 до 200 символів")]
        [RegularExpression(@"^[\p{L}0-9\s\-\.,'""!]+$",
            ErrorMessage = "Назва містить недопустимі спецсимволи")]
        public String Name { get; set; } = null!;

        // Опис: необов'язковий (nullable); якщо передається — перевіряється довжина
        [FromForm(Name = "product-description")]
        [StringLength(2000, ErrorMessage = "Опис не може перевищувати 2000 символів")]
        public String? Description { get; set; } = null;

        // Slug: необов'язковий; якщо передається — перевіряється url-коректність
        // Унікальність перевіряється в DataAccessor.IsProductFormModelValidAsync
        [FromForm(Name = "product-slug")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Slug повинен містити від 2 до 100 символів")]
        [RegularExpression(@"^[a-z0-9\-]+$",
            ErrorMessage = "Slug може містити лише малі латинські літери, цифри та дефіси")]
        public String? Slug { get; set; } = null;

        [FromForm(Name = "product-img")]
        public IFormFile? Image { get; set; } = null!;

        [FromForm(Name = "product-hidden")]
        public int IsHidden { get; set; } = 0;

        [FromForm(Name = "product-order")]
        public int Order { get; set; }

        // Кількість: ціле число > 0 або рівно -1 (означає "необмежено")
        [FromForm(Name = "product-stock")]
        [StockValidation(ErrorMessage = "Кількість повинна бути цілим позитивним числом або -1")]
        public int Stock { get; set; }

        // Ціна: позитивне число більше за 0.01
        [FromForm(Name = "product-price")]
        [Range(0.01, double.MaxValue,
            ErrorMessage = "Ціна повинна бути більше за 0.01")]
        public double Price { get; set; }
    }

    /// <summary>
    /// Кастомний атрибут валідації для поля Stock:
    /// допустимі значення — ціле додатне число (>= 1) або -1.
    /// </summary>
    public class StockValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is int stock)
            {
                return stock == -1 || stock >= 1;
            }
            return false;
        }
    }
}
