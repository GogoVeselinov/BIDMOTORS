using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Models.Entities;

namespace Project.Data.Configurations
{
    public class PriceSettingsConfiguration : IEntityTypeConfiguration<PriceSettings>
    {
        public void Configure(EntityTypeBuilder<PriceSettings> builder)
        {
            builder.HasKey(p => p.Id);
            
            builder.Property(p => p.LaborCostPerHour)
                .HasPrecision(18, 2)
                .IsRequired();
            
            builder.Property(p => p.PartsMarkupPercent)
                .HasPrecision(5, 2)
                .IsRequired();
            
            builder.Property(p => p.VATPercent)
                .HasPrecision(5, 2)
                .IsRequired();
            
            builder.Property(p => p.DiagnosticFee)
                .HasPrecision(18, 2)
                .IsRequired();
            
            // Seed default settings
            builder.HasData(new PriceSettings
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                LaborCostPerHour = 50.00m,
                PartsMarkupPercent = 20.00m,
                VATPercent = 20.00m,
                DiagnosticFee = 30.00m,
                CompanyName = "BidMotors",
                CompanyAddress = "София, България",
                CompanyPhone = "+359 888 123 456",
                CompanyEmail = "office@bidmotors.bg",
                IsActive = true,
                CreatedOn = DateTime.UtcNow
            });
        }
    }
}
