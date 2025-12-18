using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models.Entities;

namespace Project.Areas.Admin.Services
{
    public class AdminPartService
    {
        private readonly ApplicationDbContext _context;

        public AdminPartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Part>> GetAllAsync()
        {
            return await _context.Parts
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Part?> GetByIdAsync(Guid id)
        {
            return await _context.Parts.FindAsync(id);
        }

        public async Task<bool> CreateAsync(Part part)
        {
            try
            {
                part.CreatedOn = DateTime.UtcNow;
                _context.Parts.Add(part);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Part part)
        {
            try
            {
                _context.Parts.Update(part);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var part = await _context.Parts.FindAsync(id);
                if (part == null) return false;

                _context.Parts.Remove(part);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Part>> SearchAsync(string? searchTerm)
        {
            var query = _context.Parts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchTerm) ||
                    p.Description!.Contains(searchTerm) ||
                    p.Category!.Contains(searchTerm) ||
                    p.CarBrand!.Contains(searchTerm) ||
                    p.CarModel!.Contains(searchTerm));
            }

            return await query.OrderBy(p => p.Name).ToListAsync();
        }

        public async Task<List<Part>> GetFilteredAsync(
            string? name,
            string? category,
            string? oem,
            string? manufacturer,
            string? brand,
            string? model,
            string? isActive,
            string? stock)
        {
            var query = _context.Parts.AsQueryable();

            // Filter by name
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }

            // Filter by category
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category != null && p.Category.Contains(category));
            }

            // Filter by OEM number
            if (!string.IsNullOrWhiteSpace(oem))
            {
                query = query.Where(p => p.OemNumber != null && p.OemNumber.Contains(oem));
            }

            // Filter by manufacturer
            if (!string.IsNullOrWhiteSpace(manufacturer))
            {
                query = query.Where(p => p.Manufacturer != null && p.Manufacturer.Contains(manufacturer));
            }

            // Filter by car brand
            if (!string.IsNullOrWhiteSpace(brand))
            {
                query = query.Where(p => p.CarBrand != null && p.CarBrand.Contains(brand));
            }

            // Filter by car model
            if (!string.IsNullOrWhiteSpace(model))
            {
                query = query.Where(p => p.CarModel != null && p.CarModel.Contains(model));
            }

            // Filter by active status
            if (!string.IsNullOrWhiteSpace(isActive))
            {
                if (bool.TryParse(isActive, out bool activeStatus))
                {
                    query = query.Where(p => p.IsActive == activeStatus);
                }
            }

            // Filter by stock availability
            if (!string.IsNullOrWhiteSpace(stock))
            {
                switch (stock.ToLower())
                {
                    case "instock":
                        query = query.Where(p => p.StockQuantity > 5);
                        break;
                    case "lowstock":
                        query = query.Where(p => p.StockQuantity > 0 && p.StockQuantity <= 5);
                        break;
                    case "outofstock":
                        query = query.Where(p => p.StockQuantity == 0);
                        break;
                }
            }

            return await query.OrderBy(p => p.Name).ToListAsync();
        }
    }
}
