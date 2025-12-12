using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Models.Enum;

namespace Project.Models.Entities
{
    public class Service : BaseEntity
    {
        // Връзки
        public Guid ServiceRequestId { get; set; }
        public ServiceRequest ServiceRequest { get; set; }

        public Guid ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; }

        // Статус и работа
        public ServiceStatus Status { get; set; }

        public Guid? AssignedEmployeeId { get; set; }
        public Employee AssignedEmployee { get; set; }

        // Бележки
        public string? Notes { get; set; }
        public string? Result { get; set; }

        // Дати
        public DateTime? StartedOn { get; set; }
        public DateTime? CompletedOn { get; set; }

        // Навигации
        public ICollection<ServiceTask> Tasks { get; set; } = new List<ServiceTask>();
        public ICollection<ServicePartLink> PartLinks { get; set; } = new List<ServicePartLink>();
    }

}