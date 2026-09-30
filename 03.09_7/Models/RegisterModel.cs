using System.ComponentModel.DataAnnotations;

namespace AspHomework.Models;

public class RegisterModel
{
    [Required(ErrorMessage = "Поле Телефон є обов'язковим.")]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Телефон повинен складатись з 10 цифр і починатися з 0 (наприклад: 0987654321)")]
    [Display(Name = "Телефон")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Поле Пароль є обов'язковим.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Пароль повинен бути від 8 до 100 символів.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
        ErrorMessage = "Пароль повинен містити хоча б одну велику літеру, одну малу літеру, одну цифру та один спеціальний символ.")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Підтвердження паролю")]
    [Compare("Password", ErrorMessage = "Паролі не співпадають.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
