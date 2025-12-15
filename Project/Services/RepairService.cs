using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models.Entities;

namespace Project.Services
{
    public class RepairService
    {
        private readonly ApplicationDbContext _context;

        public RepairService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Създава Repair и Service от ServiceRequest и обновява статуса на заявката на InProgress
        /// </summary>
        public async Task<Repair?> CreateFromServiceRequestAsync(Guid serviceRequestId)
        {
            var serviceRequest = await _context.ServiceRequests
                .Include(sr => sr.Client)
                .Include(sr => sr.Car)
                .FirstOrDefaultAsync(sr => sr.Id == serviceRequestId);

            if (serviceRequest == null)
                return null;

            // Проверка дали вече има създаден ремонт
            if (serviceRequest.LinkedRepairId != null)
            {
                return await _context.Repairs.FindAsync(serviceRequest.LinkedRepairId);
            }

            // Намиране или създаване на ServiceType
            var serviceType = await _context.ServiceTypes
                .FirstOrDefaultAsync(st => st.Name.ToLower() == serviceRequest.ServiceType.ToLower());

            // Ако не е намерен, създай нов ServiceType автоматично
            if (serviceType == null)
            {
                serviceType = new ServiceType
                {
                    Name = serviceRequest.ServiceType,
                    Description = $"Автоматично създаден от заявка {serviceRequest.Id.ToString().Substring(0, 8)}",
                    CreatedOn = DateTime.UtcNow
                };
                _context.ServiceTypes.Add(serviceType);
                await _context.SaveChangesAsync(); // Запазваме ServiceType първо за да получи Id
            }

            // Създаване на нов ремонт
            var repair = new Repair
            {
                ClientId = serviceRequest.ClientId,
                CarId = serviceRequest.CarId,
                RequestId = serviceRequest.Id,
                WorkDescription = serviceRequest.Description,
                Price = 0, // Цената ще се попълни по-късно
                Status = "Active",
                CreatedOn = DateTime.UtcNow
            };

            _context.Repairs.Add(repair);

            // Създаване на Service запис (винаги, защото вече имаме валиден ServiceType)
            var service = new Service
            {
                ServiceRequestId = serviceRequest.Id,
                ServiceTypeId = serviceType.Id,
                Status = Models.Enum.ServiceStatus.InProgress,
                StartedOn = DateTime.UtcNow,
                CreatedOn = DateTime.UtcNow
            };

            _context.Services.Add(service);

            // Обновяване на ServiceRequest статус и свързване
            serviceRequest.Status = "InProgress";
            serviceRequest.LinkedRepairId = repair.Id;

            await _context.SaveChangesAsync();

            return repair;
        }
    }
}
