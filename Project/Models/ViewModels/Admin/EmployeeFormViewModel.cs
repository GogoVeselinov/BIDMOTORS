using System.ComponentModel.DataAnnotations;

namespace Project.Models.ViewModels.Admin
{
    public class EmployeeFormViewModel
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Името е задължително")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Имейлът е задължителен")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес")]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Телефонът е задължителен")]
        [Phone(ErrorMessage = "Невалиден телефонен номер")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Ролята е задължителна")]
        [StringLength(50)]
        public string Role { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? Password { get; set; }
    }
}
