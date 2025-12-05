using System;
using System.Collections.Generic;
using System.Linq;
using Project.Models.Entities;

namespace Project.Models.ViewModels.Parts
{
    public class PartFilterModel
    {
        public string? Search { get; set; }
        public string? CarBrand { get; set; }
        public string? CarModel { get; set; }
        public int? CarYear { get; set; }
        public string? PartType { get; set; }
        public string? OEM { get; set; }
        public string? VIN { get; set; } // опционално
    }

}