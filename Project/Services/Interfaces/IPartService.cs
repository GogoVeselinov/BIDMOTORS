using Project.Models.ViewModels.Parts;

namespace Project.Services.Interfaces
{
    public interface IPartService
    {
        Task<List<PartListViewModel>> GetFilteredPartsAsync(PartFilterModel filters);
        Task<PartListViewModel?> GetPartByIdAsync(Guid id);
        Task<bool> UpdateStockAsync(Guid partId, int quantity);
    }
}