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
    }
}
