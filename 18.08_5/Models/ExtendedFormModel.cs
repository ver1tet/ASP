using System.ComponentModel.DataAnnotations;

namespace AspHomework.Models;

public class ExtendedFormModel
{
    [Display(Name = "Ім'я")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Улюблений колір")]
    public string FavoriteColor { get; set; } = "#000000";

    [Display(Name = "Улюблена кількість")]
    public int FavoriteNumber { get; set; }

    [Display(Name = "Дата народження")]
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; } = DateTime.Today;

    [Display(Name = "Підписатися на новини?")]
    public bool Subscribe { get; set; }

    [Display(Name = "Рівень досвіду")]
    public string ExperienceLevel { get; set; } = "Junior"; // Junior, Middle, Senior
}
