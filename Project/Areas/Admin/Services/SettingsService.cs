using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models.Entities;

namespace Project.Areas.Admin.Services
{
    public class SettingsService
    {
        private readonly ApplicationDbContext _context;

        public SettingsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PriceSettings> GetActiveSettingsAsync()
        {
            var settings = await _context.PriceSettings
                .FirstOrDefaultAsync(s => s.IsActive);

            if (settings == null)
            {
                // Create default settings if none exist
                settings = new PriceSettings
                {
                    Id = Guid.NewGuid(),
                    LaborCostPerHour = 50.00m,
                    PartsMarkupPercent = 20.00m,
                    VATPercent = 20.00m,
                    DiagnosticFee = 30.00m,
                    CompanyName = "BidMotors",
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow
                };

                _context.PriceSettings.Add(settings);
                await _context.SaveChangesAsync();
            }

            return settings;
        }

        public async Task<bool> UpdateSettingsAsync(PriceSettings settings)
        {
            try
            {
                var existing = await _context.PriceSettings
                    .FirstOrDefaultAsync(s => s.Id == settings.Id);

                if (existing == null)
                {
                    return false;
                }

                existing.LaborCostPerHour = settings.LaborCostPerHour;
                existing.PartsMarkupPercent = settings.PartsMarkupPercent;
                existing.VATPercent = settings.VATPercent;
                existing.DiagnosticFee = settings.DiagnosticFee;
                existing.CompanyName = settings.CompanyName;
                existing.CompanyAddress = settings.CompanyAddress;
                existing.CompanyPhone = settings.CompanyPhone;
                existing.CompanyEmail = settings.CompanyEmail;
                existing.CompanyVATNumber = settings.CompanyVATNumber;
                existing.CompanyRegistrationNumber = settings.CompanyRegistrationNumber;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
