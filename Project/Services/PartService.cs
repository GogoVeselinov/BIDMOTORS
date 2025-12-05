using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models.Entities;
using Project.Models.ViewModels.Parts;
using Project.Services.Interfaces;

namespace Project.Services
{
    public class PartService : IPartService
    {
        private readonly ApplicationDbContext _context;

        public PartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PartListViewModel>> GetFilteredPartsAsync(PartFilterModel filters)
        {
            var query = _context.Parts.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                query = query.Where(p => p.Name.Contains(filters.Search) ||
                                        (p.Description != null && p.Description.Contains(filters.Search)) ||
                                        (p.OEM != null && p.OEM.Contains(filters.Search)));
            }

            if (!string.IsNullOrWhiteSpace(filters.CarBrand))
            {
                query = query.Where(p => p.CarBrand != null && p.CarBrand.Contains(filters.CarBrand));
            }

            if (!string.IsNullOrWhiteSpace(filters.CarModel))
            {
                query = query.Where(p => p.CarModel != null && p.CarModel.Contains(filters.CarModel));
            }

            if (filters.CarYear.HasValue)
            {
                query = query.Where(p => p.CarYear == filters.CarYear.Value);
            }

            if (!string.IsNullOrWhiteSpace(filters.PartType))
            {
                query = query.Where(p => p.PartType != null && p.PartType.Contains(filters.PartType));
            }

            if (!string.IsNullOrWhiteSpace(filters.OEM))
            {
                query = query.Where(p => p.OEM != null && p.OEM.Contains(filters.OEM));
            }

            var parts = await query
                .Select(p => new PartListViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Category = p.Category,
                    PartType = p.PartType,
                    CarBrand = p.CarBrand,
                    CarModel = p.CarModel,
                    CarYear = p.CarYear,
                    OEM = p.OEM,
                    Price = p.Price,
                    QuantityInStock = p.QuantityInStock,
                    Supplier = p.Supplier,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();

            return parts;
        }

        public async Task<PartListViewModel?> GetPartByIdAsync(Guid id)
        {
            var part = await _context.Parts.FindAsync(id);
            if (part == null) return null;

            return new PartListViewModel
            {
                Id = part.Id,
                Name = part.Name,
                Description = part.Description,
                Category = part.Category,
                PartType = part.PartType,
                CarBrand = part.CarBrand,
                CarModel = part.CarModel,
                CarYear = part.CarYear,
                OEM = part.OEM,
                Price = part.Price,
                QuantityInStock = part.QuantityInStock,
                Supplier = part.Supplier,
                ImageUrl = part.ImageUrl
            };
        }

        public async Task<bool> UpdateStockAsync(Guid partId, int quantity)
        {
            var part = await _context.Parts.FindAsync(partId);
            if (part == null) return false;

            part.QuantityInStock = quantity;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
