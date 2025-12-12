using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models.Entities
{
    public class ServiceTask : BaseEntity
    {
        public Guid ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        public string Title { get; set; } = string.Empty;   // "Проверка на вакуум"
        public bool IsCompleted { get; set; }

        public DateTime? CompletedOn { get; set; }
        public Guid? CompletedByEmployeeId { get; set; }
        public Employee? CompletedByEmployee { get; set; }

        public string? Notes { get; set; }
    }
}