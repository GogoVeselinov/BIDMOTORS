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
                LaborHours = 0,
                LaborCost = 0,
                PartsCost = 0,
                TotalCost = 0,
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

        /// <summary>
        /// Обновява работни часове и преизчислява цените
        /// </summary>
        public async Task<bool> UpdateLaborHoursAsync(Guid repairId, decimal hours)
        {
            var repair = await _context.Repairs.FindAsync(repairId);
            if (repair == null) return false;

            var settings = await _context.PriceSettings.FirstOrDefaultAsync(s => s.IsActive);
            if (settings == null) return false;

            repair.LaborHours = hours;
            repair.LaborCost = hours * settings.LaborCostPerHour;

            await RecalculateTotalCostAsync(repair, settings);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Добавя част към ремонт
        /// </summary>
        public async Task<bool> AddPartToRepairAsync(Guid repairId, Guid partId, int quantity)
        {
            var repair = await _context.Repairs
                .Include(r => r.UsedParts)
                .FirstOrDefaultAsync(r => r.Id == repairId);

            if (repair == null) return false;

            var part = await _context.Parts.FindAsync(partId);
            if (part == null || part.StockQuantity < quantity) return false;

            var settings = await _context.PriceSettings.FirstOrDefaultAsync(s => s.IsActive);
            if (settings == null) return false;

            // Прилагане на надценка върху цената на частта
            var priceWithMarkup = part.Price * (1 + settings.PartsMarkupPercent / 100);

            var usedPart = new UsedPart
            {
                RepairId = repairId,
                PartId = partId,
                QuantityUsed = quantity,
                UnitPriceAtMoment = priceWithMarkup,
                TotalPrice = priceWithMarkup * quantity,
                CreatedOn = DateTime.UtcNow
            };

            _context.UsedParts.Add(usedPart);

            // Намаляване на количеството в склада
            part.StockQuantity -= quantity;

            await RecalculatePartsCostAsync(repair);
            await RecalculateTotalCostAsync(repair, settings);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Преизчислява цената на частите
        /// </summary>
        private async Task RecalculatePartsCostAsync(Repair repair)
        {
            var usedParts = await _context.UsedParts
                .Where(up => up.RepairId == repair.Id)
                .ToListAsync();

            repair.PartsCost = usedParts.Sum(up => up.TotalPrice);
        }

        /// <summary>
        /// Преизчислява общата цена с ДДС
        /// </summary>
        private async Task RecalculateTotalCostAsync(Repair repair, PriceSettings settings)
        {
            var subtotal = repair.LaborCost + repair.PartsCost;
            var vatAmount = subtotal * (settings.VATPercent / 100);
            repair.TotalCost = subtotal + vatAmount;
            repair.Price = repair.TotalCost; // Sync Price with TotalCost
        }

        /// <summary>
        /// Завършва ремонт и подготвя за фактуриране
        /// </summary>
        public async Task<bool> CompleteRepairAsync(Guid repairId)
        {
            var repair = await _context.Repairs
                .Include(r => r.ServiceRequest)
                .FirstOrDefaultAsync(r => r.Id == repairId);

            if (repair == null) return false;

            repair.Status = "Completed";
            repair.FinishedOn = DateTime.UtcNow;

            // Генериране на номер на фактура
            var invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{repairId.ToString().Substring(0, 8).ToUpper()}";
            repair.InvoiceNumber = invoiceNumber;

            // Обновяване на ServiceRequest статус
            if (repair.ServiceRequest != null)
            {
                repair.ServiceRequest.Status = "Completed";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Връща детайли за ремонт с всички свързани данни
        /// </summary>
        public async Task<Repair?> GetRepairDetailsAsync(Guid repairId)
        {
            return await _context.Repairs
                .Include(r => r.Client)
                .Include(r => r.Car)
                .Include(r => r.ServiceRequest)
                .Include(r => r.UsedParts)
                    .ThenInclude(up => up.Part)
                .FirstOrDefaultAsync(r => r.Id == repairId);
        }
    }
}
