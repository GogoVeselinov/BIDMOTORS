using System.Collections.Generic;

namespace Project.Models.ViewModels.Parts
{
    public class PartsPageViewModel
    {
        public List<PartListViewModel> Parts { get; set; } = new();
        public PartFilterModel Filters { get; set; } = new();
    }
}
