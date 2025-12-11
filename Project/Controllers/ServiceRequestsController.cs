using Microsoft.AspNetCore.Mvc;
using Project.Data;
using Project.Models.Entities;
using Project.Models.ViewModels.ServiceRequests;
using Project.Services;

namespace Project.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly ServiceRequestService _serviceRequestService;
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notificationService;

        public ServiceRequestsController(
            ServiceRequestService serviceRequestService,
            ApplicationDbContext context,
            NotificationService notificationService)
        {
            _serviceRequestService = serviceRequestService;
            _context = context;
            _notificationService = notificationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View(new CreateServiceRequestViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateServiceRequestViewModel viewModel)
        {
            // Добавяме debugging
            Console.WriteLine($"=== CREATE SERVICE REQUEST DEBUG ===");
            Console.WriteLine($"=== RAW FORM DATA ===");
            foreach (var key in Request.Form.Keys)
            {
                Console.WriteLine($"{key}: {Request.Form[key]}");
            }
            Console.WriteLine($"=== MODEL BINDING RESULT ===");
            Console.WriteLine($"ClientName: '{viewModel.ClientName}'");
            Console.WriteLine($"Phone: '{viewModel.Phone}'");
            Console.WriteLine($"Email: '{viewModel.Email}'");
            Console.WriteLine($"Brand: '{viewModel.Brand}'");
            Console.WriteLine($"Model: '{viewModel.Model}'");
            Console.WriteLine($"Year: {viewModel.Year}");
            Console.WriteLine($"RegistrationNumber: '{viewModel.RegistrationNumber}'");
            Console.WriteLine($"VIN: '{viewModel.VIN}'");
            Console.WriteLine($"ServiceType: '{viewModel.ServiceType}'");
            Console.WriteLine($"Description: '{viewModel.Description}'");
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
            
            if (!ModelState.IsValid)
            {
                // Показваме конкретните грешки
                var errors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                Console.WriteLine($"ModelState Errors: {errors}");
                TempData["ErrorMessage"] = $"Моля, попълнете всички задължителни полета правилно. Грешки: {errors}";
                return View(viewModel);
            }

            try
            {
                // Проверка дали клиент с този телефон вече съществува
                var existingClient = _context.Clients.FirstOrDefault(c => c.Phone == viewModel.Phone);
                Client client;

                if (existingClient != null)
                {
                    Console.WriteLine($"Found existing client: {existingClient.Id}");
                    client = existingClient;
                    // Актуализираме имейла, ако е променен
                    if (!string.IsNullOrEmpty(viewModel.Email) && client.Email != viewModel.Email)
                    {
                        client.Email = viewModel.Email;
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    // Създаваме нов клиент
                    Console.WriteLine("Creating new client...");
                    client = new Client
                    {
                        Name = viewModel.ClientName,
                        Phone = viewModel.Phone,
                        Email = viewModel.Email ?? string.Empty,
                        PasswordHash = null // За гост клиенти
                    };
                    _context.Clients.Add(client);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"New client created with ID: {client.Id}");
                }

                // Проверка дали кола с този рег. номер вече съществува
                Console.WriteLine($"Checking for existing car with RegistrationNumber: {viewModel.RegistrationNumber}");
                var existingCar = _context.Cars
                    .Where(c => c.RegistrationNumber == viewModel.RegistrationNumber)
                    .FirstOrDefault();
                
                Car car;

                if (existingCar != null)
                {
                    Console.WriteLine($"Found existing car: {existingCar.Id}, updating data...");
                    car = existingCar;
                    
                    // Актуализираме данните на колата
                    bool needsUpdate = false;
                    
                    if (car.Brand != viewModel.Brand)
                    {
                        car.Brand = viewModel.Brand;
                        needsUpdate = true;
                    }
                    
                    if (car.Model != viewModel.Model)
                    {
                        car.Model = viewModel.Model;
                        needsUpdate = true;
                    }
                    
                    if (car.Year != viewModel.Year)
                    {
                        car.Year = viewModel.Year;
                        needsUpdate = true;
                    }
                    
                    if (!string.IsNullOrEmpty(viewModel.VIN) && car.VIN != viewModel.VIN)
                    {
                        car.VIN = viewModel.VIN;
                        needsUpdate = true;
                    }
                    
                    if (car.ClientId != client.Id)
                    {
                        car.ClientId = client.Id;
                        needsUpdate = true;
                    }
                    
                    if (needsUpdate)
                    {
                        Console.WriteLine("Updating car data...");
                        await _context.SaveChangesAsync();
                        Console.WriteLine("Car updated successfully");
                    }
                    else
                    {
                        Console.WriteLine("No changes needed for car");
                    }
                }
                else
                {
                    // Създаваме нова кола
                    Console.WriteLine("Creating new car...");
                    car = new Car
                    {
                        Brand = viewModel.Brand,
                        Model = viewModel.Model,
                        Year = viewModel.Year,
                        RegistrationNumber = viewModel.RegistrationNumber,
                        VIN = viewModel.VIN,
                        ClientId = client.Id
                    };
                    
                    _context.Cars.Add(car);
                    
                    try
                    {
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"New car created with ID: {car.Id}");
                    }
                    catch (Exception carEx)
                    {
                        Console.WriteLine($"Error creating car: {carEx.Message}");
                        
                        // Може би друг потребител е създал колата междувременно
                        // Опитваме да я намерим отново
                        existingCar = _context.Cars
                            .Where(c => c.RegistrationNumber == viewModel.RegistrationNumber)
                            .FirstOrDefault();
                            
                        if (existingCar != null)
                        {
                            Console.WriteLine($"Car was created by another request, using existing car ID: {existingCar.Id}");
                            car = existingCar;
                        }
                        else
                        {
                            throw; // Ако все още не можем да я намерим, хвърляме грешката
                        }
                    }
                }

                // Създаваме заявката за услуга
                Console.WriteLine("Creating service request...");
                var serviceRequest = new ServiceRequest
                {
                    ClientId = client.Id,
                    CarId = car.Id,
                    ServiceType = viewModel.ServiceType,
                    Description = viewModel.Description,
                    Status = "Pending"
                };
                _context.ServiceRequests.Add(serviceRequest);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Service request created with ID: {serviceRequest.Id}");

                // Създаваме известие за всички служители
                var employees = _context.Employees.ToList();
                foreach (var employee in employees)
                {
                    await _notificationService.CreateNotificationForEmployee(
                        employee.Id,
                        $"Нова заявка от {client.Name}",
                        $"Получена е нова заявка за {viewModel.ServiceType}",
                        serviceRequest.Id
                    );
                }

                // Автоматично "логваме" клиента за да може да вижда заявките си
                HttpContext.Session.SetString("UserId", client.Id.ToString());
                HttpContext.Session.SetString("UserType", "Client");
                Console.WriteLine($"Client auto-logged in with ID: {client.Id}");

                TempData["SuccessMessage"] = "Вашата заявка е изпратена успешно! Можете да проследите статуса й от Моите заявки.";
                return RedirectToAction("MyRequests");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Възникна грешка при обработката на заявката: {ex.Message}";
                return View(viewModel);
            }
        }

        // Моите заявки
        public IActionResult MyRequests()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            var userType = HttpContext.Session.GetString("UserType");

            if (string.IsNullOrEmpty(userIdString) || string.IsNullOrEmpty(userType) || !Guid.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Само клиенти могат да виждат своите заявки
            if (userType != "Client")
            {
                TempData["ErrorMessage"] = "Само клиенти имат достъп до заявките";
                return RedirectToAction("Index", "Home");
            }

            var requests = _serviceRequestService.GetClientRequests(userId);
            return View(requests);
        }

        // Детайли за заявка (partial view)
        public IActionResult Details(Guid id)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            var userType = HttpContext.Session.GetString("UserType");

            if (string.IsNullOrEmpty(userIdString) || string.IsNullOrEmpty(userType) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            if (userType != "Client")
            {
                return Unauthorized();
            }

            var details = _serviceRequestService.GetRequestDetails(id, userId);

            if (details == null)
            {
                return NotFound();
            }

            return PartialView("_RequestDetails", details);
        }
    }
}
