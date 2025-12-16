using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models.Entities
{
    public class PriceSettings : BaseEntity
    {
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999.99)]
        public decimal LaborCostPerHour { get; set; } = 50.00m;
        
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100)]
        public decimal PartsMarkupPercent { get; set; } = 20.00m;
        
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100)]
        public decimal VATPercent { get; set; } = 20.00m;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999.99)]
        public decimal DiagnosticFee { get; set; } = 30.00m;
        
        [StringLength(100)]
        public string? CompanyName { get; set; } = "BidMotors";
        
        [StringLength(500)]
        public string? CompanyAddress { get; set; }
        
        [StringLength(50)]
        public string? CompanyPhone { get; set; }
        
        [StringLength(100)]
        public string? CompanyEmail { get; set; }
        
        [StringLength(50)]
        public string? CompanyVATNumber { get; set; }
        
        [StringLength(50)]
        public string? CompanyRegistrationNumber { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
