using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models.ViewModels.ServiceRequests;

namespace Project.Services
{
    public class ServiceRequestService
    {
        private readonly ApplicationDbContext _db;

        public ServiceRequestService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Вземане на всички заявки за клиент
        public List<MyRequestsViewModel> GetClientRequests(Guid clientId)
        {
            return _db.ServiceRequests
                .Include(sr => sr.Car)
                .Where(sr => sr.ClientId == clientId)
                .OrderByDescending(sr => sr.CreatedOn)
                .Select(sr => new MyRequestsViewModel
                {
                    Id = sr.Id,
                    ServiceTypeName = sr.ServiceType,
                    Status = sr.Status,
                    CreatedOn = sr.CreatedOn,
                    CarInfo = $"{sr.Car.Brand} {sr.Car.Model} ({sr.Car.Year})",
                    Description = sr.Description
                })
                .ToList();
        }

        // Вземане на детайли за заявка
        public RequestDetailsViewModel? GetRequestDetails(Guid requestId, Guid clientId)
        {
            var request = _db.ServiceRequests
                .Include(sr => sr.Car)
                .Include(sr => sr.Client)
                .Include(sr => sr.LinkedRepair)
                .FirstOrDefault(sr => sr.Id == requestId && sr.ClientId == clientId);

            if (request == null)
                return null;

            return new RequestDetailsViewModel
            {
                Id = request.Id,
                ServiceTypeName = request.ServiceType,
                Status = request.Status,
                CreatedOn = request.CreatedOn,
                Description = request.Description,
                
                // Car details
                CarMake = request.Car.Brand,
                CarModel = request.Car.Model,
                CarYear = request.Car.Year,
                LicensePlate = request.Car.RegistrationNumber,
                
                // Client details
                ClientName = request.Client.Name,
                ClientPhone = request.Client.Phone,
                ClientEmail = request.Client.Email,
                
                // Repair details
                HasRepair = request.LinkedRepair != null,
                RepairId = request.LinkedRepair?.Id,
                RepairPrice = request.LinkedRepair?.Price,
                RepairWorkDescription = request.LinkedRepair?.WorkDescription,
                RepairFinishedOn = request.LinkedRepair?.FinishedOn
            };
        }
    }
}
