using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models.Entities
{
    public class ServicePartLink : BaseEntity
    {
        public Guid ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        public string Title { get; set; } = string.Empty;        // "Дебитомер"
        public string Url { get; set; } = string.Empty;          // https://intercars...
        public string? Supplier { get; set; }    // InterCars / Elit

        public string? Notes { get; set; }
    }

}