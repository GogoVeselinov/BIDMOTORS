using System.ComponentModel.DataAnnotations;

namespace Project.Models.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Името и фамилията са задължителни")]
        [StringLength(200, ErrorMessage = "Името не може да бъде по-дълго от 200 символа")]
        [Display(Name = "Име и фамилия")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Имейлът е задължителен")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес")]
        [StringLength(255, ErrorMessage = "Имейлът не може да бъде по-дълъг от 255 символа")]
        [Display(Name = "Имейл")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Паролата е задължителна")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Паролата трябва да бъде между 6 и 100 символа")]
        [DataType(DataType.Password)]
        [Display(Name = "Парола")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Потвърждението на паролата е задължително")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролите не съвпадат")]
        [Display(Name = "Потвърди парола")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
