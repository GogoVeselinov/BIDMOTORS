using System.ComponentModel.DataAnnotations;

namespace Project.Models.ViewModels.ServiceRequests
{
    public class CreateServiceRequestViewModel
    {
        // Стъпка 1: Данни за клиента
        [Required(ErrorMessage = "Името е задължително")]
        [StringLength(200)]
        public string ClientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Телефонът е задължителен")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Невалиден имейл адрес")]
        [StringLength(200)]
        public string? Email { get; set; }

        // Стъпка 2: Данни за автомобила
        [Required(ErrorMessage = "Марката е задължителна")]
        [StringLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Моделът е задължителен")]
        [StringLength(100)]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "Годината е задължителна")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Годината трябва да е 4 цифри")]
        [RegularExpression(@"^(19|20)\d{2}$", ErrorMessage = "Невалидна година")]
        public string Year { get; set; } = string.Empty;

        [Required(ErrorMessage = "Регистрационният номер е задължителен")]
        [StringLength(20)]
        public string RegistrationNumber { get; set; } = string.Empty;

        [StringLength(17)]
        public string? VIN { get; set; }

        // Стъпка 3: Данни за услугата
        [Required(ErrorMessage = "Видът услуга е задължителен")]
        [StringLength(50)]
        public string ServiceType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Описанието е задължително")]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;
    }
}
