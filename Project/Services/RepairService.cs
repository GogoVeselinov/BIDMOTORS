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

            // Изчисляване на текущата сума ПРЕДИ да добавим новата част
            var currentPartsCost = repair.UsedParts.Sum(up => up.TotalPrice);

            _context.UsedParts.Add(usedPart);

            // Намаляване на количеството в склада
            part.StockQuantity -= quantity;

            // Добавяне само на новата цена към съществуващата
            repair.PartsCost = currentPartsCost + usedPart.TotalPrice;

            Console.WriteLine($"[AddPart] Part: {part.Name}, Base: {part.Price}, WithMarkup: {priceWithMarkup}, Qty: {quantity}, Total: {usedPart.TotalPrice}");
            Console.WriteLine($"[AddPart] UsedParts count: {repair.UsedParts.Count}, PartsCost: {repair.PartsCost}");

            // Преизчисляване на общата цена с ДДС
            var subtotal = repair.LaborCost + repair.PartsCost;
            var vatAmount = subtotal * (settings.VATPercent / 100);
            repair.TotalCost = subtotal + vatAmount;
            repair.Price = repair.TotalCost;

            Console.WriteLine($"[AddPart] Subtotal: {subtotal}, VAT: {vatAmount}, Total: {repair.TotalCost}");

            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Премахва част от ремонт
        /// </summary>
        public async Task<bool> RemovePartFromRepairAsync(Guid usedPartId)
        {
            var usedPart = await _context.UsedParts
                .Include(up => up.Part)
                .FirstOrDefaultAsync(up => up.Id == usedPartId);

            if (usedPart == null) return false;

            var repair = await _context.Repairs
                .Include(r => r.UsedParts)
                .FirstOrDefaultAsync(r => r.Id == usedPart.RepairId);

            if (repair == null) return false;

            var settings = await _context.PriceSettings.FirstOrDefaultAsync(s => s.IsActive);
            if (settings == null) return false;

            Console.WriteLine($"[RemovePart] Part: {usedPart.Part?.Name}, Qty: {usedPart.QuantityUsed}, Total: {usedPart.TotalPrice}");

            // Връщане на количеството в склада
            if (usedPart.Part != null)
            {
                usedPart.Part.StockQuantity += usedPart.QuantityUsed;
            }

            // Премахване на частта от ремонта
            _context.UsedParts.Remove(usedPart);

            // Изчисляване на текущата сума СЛЕД като премахнем частта
            repair.PartsCost = repair.UsedParts.Where(up => up.Id != usedPartId).Sum(up => up.TotalPrice);

            // Преизчисляване на общата цена с ДДС
            var subtotal = repair.LaborCost + repair.PartsCost;
            var vatAmount = subtotal * (settings.VATPercent / 100);
            repair.TotalCost = subtotal + vatAmount;
            repair.Price = repair.TotalCost;

            Console.WriteLine($"[RemovePart] New PartsCost: {repair.PartsCost}, TotalCost: {repair.TotalCost}");

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

        /// <summary>
        /// Преизчислява всички цени за съществуващ ремонт (за поправка на стари данни)
        /// </summary>
        public async Task<bool> RecalculateRepairCostsAsync(Guid repairId)
        {
            var repair = await _context.Repairs
                .Include(r => r.UsedParts)
                .FirstOrDefaultAsync(r => r.Id == repairId);

            if (repair == null) return false;

            var settings = await _context.PriceSettings.FirstOrDefaultAsync(s => s.IsActive);
            if (settings == null) return false;

            Console.WriteLine($"[Recalculate] Repair ID: {repairId}");
            Console.WriteLine($"[Recalculate] UsedParts count: {repair.UsedParts.Count}");
            
            // Зареждане на Part данните
            await _context.Entry(repair).Collection(r => r.UsedParts).Query().Include(up => up.Part).LoadAsync();
            
            // Показване на всички части
            foreach (var up in repair.UsedParts)
            {
                var partName = up.Part?.Name ?? "Unknown";
                Console.WriteLine($"[Recalculate] Part: ID={up.Id.ToString().Substring(0,8)}, PartName={partName}, PartId={up.PartId.ToString().Substring(0,8)}, Qty={up.QuantityUsed}, Unit={up.UnitPriceAtMoment}, Total={up.TotalPrice}");
            }
            
            // Преизчисляване на цената на частите от данните в базата
            repair.PartsCost = repair.UsedParts.Sum(up => up.TotalPrice);
            Console.WriteLine($"[Recalculate] PartsCost: {repair.PartsCost}");

            // Изчисляване на цена за труд
            repair.LaborCost = repair.LaborHours * settings.LaborCostPerHour;
            Console.WriteLine($"[Recalculate] LaborCost: {repair.LaborCost}");

            // Изчисляване на обща цена с ДДС
            var subtotal = repair.LaborCost + repair.PartsCost;
            var vatAmount = subtotal * (settings.VATPercent / 100);
            repair.TotalCost = subtotal + vatAmount;
            repair.Price = repair.TotalCost;

            Console.WriteLine($"[Recalculate] Subtotal: {subtotal}, VAT: {vatAmount}, Total: {repair.TotalCost}");

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Премахва дублирани части от ремонт
        /// </summary>
        public async Task<bool> RemoveDuplicatePartsAsync(Guid repairId)
        {
            var repair = await _context.Repairs
                .Include(r => r.UsedParts)
                    .ThenInclude(up => up.Part)
                .FirstOrDefaultAsync(r => r.Id == repairId);

            if (repair == null) return false;

            Console.WriteLine($"[RemoveDuplicates] Total UsedParts: {repair.UsedParts.Count}");

            // Групиране по PartId за намиране на дубликати
            var duplicateGroups = repair.UsedParts
                .GroupBy(up => up.PartId)
                .Where(g => g.Count() > 1)
                .ToList();

            foreach (var group in duplicateGroups)
            {
                var partsToKeep = group.OrderBy(up => up.CreatedOn).First(); // Запазваме първата
                var partsToRemove = group.OrderBy(up => up.CreatedOn).Skip(1).ToList(); // Изтриваме останалите

                Console.WriteLine($"[RemoveDuplicates] Part '{partsToKeep.Part?.Name}' has {group.Count()} duplicates");

                foreach (var dup in partsToRemove)
                {
                    Console.WriteLine($"[RemoveDuplicates] Removing duplicate: {dup.Id.ToString().Substring(0, 8)}");
                    _context.UsedParts.Remove(dup);
                    
                    // Връщане на количеството в склада
                    if (dup.Part != null)
                    {
                        dup.Part.StockQuantity += dup.QuantityUsed;
                    }
                }
            }

            await _context.SaveChangesAsync();
            
            // Преизчисляване след изтриване
            await RecalculateRepairCostsAsync(repairId);

            return true;
        }
    }
}
