using System.ComponentModel.DataAnnotations;

namespace Project.Models.ViewModels.Account
{
    public class ProfileViewModel
    {
        [Required]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Phone]
        public string? Phone { get; set; }
        
        public DateTime CreatedOn { get; set; }
        
        public string LastLogin { get; set; } = "Никога";
        
        public string Role { get; set; } = string.Empty;
        
        [StringLength(100, MinimumLength = 6)]
        public string? NewPassword { get; set; }
        
        [Compare("NewPassword", ErrorMessage = "Паролите не съвпадат")]
        public string? ConfirmPassword { get; set; }
    }
}
