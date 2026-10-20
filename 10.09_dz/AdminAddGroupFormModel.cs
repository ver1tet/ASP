using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ASP_P42.Models.Admin
{
    /* Д.З. 10.09.2026 - Реалізувати валідацію моделі форми
     * додавання нової товарної групи
     * - назва (довжина, відсутність спецсимволів)
     * - опис (довжина)
     * - Slug (унікальність, url-коректність)
     */
    public class AdminAddGroupFormModel
    {
        [FromForm(Name = "group-id")]
        public Guid? Id { get; set; }

        [FromForm(Name = "group-parent")]
        public Guid? ParentId { get; set; } = null!;

        // Назва: обов'язкова, довжина від 2 до 100 символів,
        // без спецсимволів (тільки літери, цифри, пробіли, дефіси)
        [FromForm(Name = "group-name")]
        [Required(ErrorMessage = "Назва групи є обов'язковою")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Назва повинна містити від 2 до 100 символів")]
        [RegularExpression(@"^[\p{L}0-9\s\-]+$",
            ErrorMessage = "Назва може містити лише літери, цифри, пробіли та дефіси")]
        public String Name { get; set; } = null!;

        // Опис: необов'язковий, але якщо переданий — не більше 1000 символів
        [FromForm(Name = "group-description")]
        [StringLength(1000, ErrorMessage = "Опис не може перевищувати 1000 символів")]
        public String Description { get; set; } = null!;

        // Slug: url-коректний (тільки малі літери, цифри, дефіси),
        // унікальність перевіряється в DataAccessor
        [FromForm(Name = "group-slug")]
        [Required(ErrorMessage = "Slug є обов'язковим")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Slug повинен містити від 2 до 100 символів")]
        [RegularExpression(@"^[a-z0-9\-]+$",
            ErrorMessage = "Slug може містити лише малі латинські літери, цифри та дефіси")]
        public String Slug { get; set; } = null!;

        [FromForm(Name = "group-img")]
        public IFormFile Image { get; set; } = null!;

        [FromForm(Name = "group-hidden")]
        public int IsHidden { get; set; } = 0;

        [FromForm(Name = "group-order")]
        public int Order { get; set; }
    }
}
