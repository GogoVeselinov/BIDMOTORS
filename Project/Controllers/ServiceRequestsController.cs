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
        private readonly EmailService _emailService;

        public ServiceRequestsController(
            ServiceRequestService serviceRequestService,
            ApplicationDbContext context,
            NotificationService notificationService,
            EmailService emailService)
        {
            _serviceRequestService = serviceRequestService;
            _context = context;
            _notificationService = notificationService;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            var viewModel = new CreateServiceRequestViewModel();
            
            // Ако потребителят е логнат, попълваме данните му автоматично
            var userIdString = HttpContext.Session.GetString("UserId");
            var userType = HttpContext.Session.GetString("UserType");
            
            if (!string.IsNullOrEmpty(userIdString) && userType == "Client" && Guid.TryParse(userIdString, out var userId))
            {
                var client = _context.Clients.FirstOrDefault(c => c.Id == userId);
                if (client != null)
                {
                    viewModel.ClientName = client.Name;
                    viewModel.Phone = client.Phone;
                    viewModel.Email = client.Email;
                    Console.WriteLine($"Pre-filled form for logged client: {client.Name}");
                }
            }
            
            return View(viewModel);
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
                // Проверяваме дали потребителят е логнат и вземаме неговото ID
                var userIdString = HttpContext.Session.GetString("UserId");
                Guid currentUserId = Guid.Empty;
                var userIsAuthenticated = !string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out currentUserId);
                
                // Проверка дали имейлът вече съществува при ДРУГ клиент
                // Игнорираме проверката ако имейлът принадлежи на логнатия потребител
                if (!string.IsNullOrEmpty(viewModel.Email))
                {
                    var clientWithEmail = _context.Clients
                        .FirstOrDefault(c => c.Email == viewModel.Email && c.Phone != viewModel.Phone);
                    
                    // Ако намерим клиент с този имейл и различен телефон
                    if (clientWithEmail != null)
                    {
                        // Проверяваме дали това НЕ Е логнатият потребител
                        if (!userIsAuthenticated || clientWithEmail.Id != currentUserId)
                        {
                            Console.WriteLine($"Email {viewModel.Email} already exists for different client {clientWithEmail.Id}");
                            TempData["ErrorMessage"] = "Този имейл адрес вече е регистриран с друг телефонен номер. Моля, използвайте друг имейл или се свържете с нас.";
                            
                            // Ако сме от бърза форма, редиректваме към Home
                            if (Request.Headers["Referer"].ToString().Contains("/Home") || 
                                Request.Headers["Referer"].ToString().Contains("/#"))
                            {
                                return RedirectToAction("Index", "Home");
                            }
                            
                            return View(viewModel);
                        }
                    }
                }

                // Проверка дали клиент с този телефон или имейл вече съществува
                var existingClient = _context.Clients.FirstOrDefault(c => 
                    c.Phone == viewModel.Phone || 
                    (!string.IsNullOrEmpty(viewModel.Email) && c.Email == viewModel.Email));
                Client client;

                if (existingClient != null)
                {
                    Console.WriteLine($"Found existing client: {existingClient.Id} (Phone: {existingClient.Phone}, Email: {existingClient.Email})");
                    client = existingClient;
                    
                    // Актуализираме имейла ако е различен и не е празен
                    if (!string.IsNullOrEmpty(viewModel.Email) && client.Email != viewModel.Email)
                    {
                        Console.WriteLine($"Updating email from '{client.Email}' to '{viewModel.Email}'");
                        client.Email = viewModel.Email;
                    }
                    
                    // Актуализираме телефона ако е различен
                    if (client.Phone != viewModel.Phone)
                    {
                        Console.WriteLine($"Updating phone from '{client.Phone}' to '{viewModel.Phone}'");
                        client.Phone = viewModel.Phone;
                    }
                    
                    // Актуализираме името ако е различно
                    if (client.Name != viewModel.ClientName)
                    {
                        Console.WriteLine($"Updating name from '{client.Name}' to '{viewModel.ClientName}'");
                        client.Name = viewModel.ClientName;
                    }
                    
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Client updated successfully");
                }
                else
                {
                    // Проверяваме дали потребителят е логнат
                    var sessionUserId = HttpContext.Session.GetString("UserId");
                    var isUserLoggedIn = !string.IsNullOrEmpty(sessionUserId);
                    
                    // Създаваме нов клиент
                    Console.WriteLine("Creating new client...");
                    client = new Client
                    {
                        Name = viewModel.ClientName,
                        Phone = viewModel.Phone,
                        Email = viewModel.Email ?? string.Empty,
                        PasswordHash = null,
                        IsGuest = !isUserLoggedIn // Маркираме като гост ако не е логнат
                    };
                    _context.Clients.Add(client);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"New client created with ID: {client.Id} (IsGuest: {client.IsGuest})");
                }

                // Проверка дали кола вече съществува
                Car? existingCar = null;
                
                if (!string.IsNullOrEmpty(viewModel.RegistrationNumber))
                {
                    Console.WriteLine($"Checking for existing car with RegistrationNumber: {viewModel.RegistrationNumber}");
                    existingCar = _context.Cars
                        .Where(c => c.RegistrationNumber == viewModel.RegistrationNumber)
                        .FirstOrDefault();
                }
                
                // Ако няма рег. номер, проверяваме по VIN или по клиент+марка+модел+година
                if (existingCar == null && !string.IsNullOrEmpty(viewModel.VIN))
                {
                    Console.WriteLine($"Checking for existing car with VIN: {viewModel.VIN}");
                    existingCar = _context.Cars.FirstOrDefault(c => c.VIN == viewModel.VIN);
                }
                
                if (existingCar == null)
                {
                    Console.WriteLine($"Checking for existing car by client+brand+model+year");
                    existingCar = _context.Cars
                        .Where(c => c.ClientId == client.Id 
                               && c.Brand == viewModel.Brand 
                               && c.Model == viewModel.Model 
                               && c.Year == viewModel.Year)
                        .FirstOrDefault();
                }
                
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
                    // Генерираме временен рег. номер ако няма
                    var registrationNumber = string.IsNullOrEmpty(viewModel.RegistrationNumber)
                        ? $"{viewModel.Brand}-{viewModel.Model}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}"
                        : viewModel.RegistrationNumber;
                    
                    // Създаваме нова кола
                    Console.WriteLine($"Creating new car with RegistrationNumber: {registrationNumber}");
                    car = new Car
                    {
                        Brand = viewModel.Brand,
                        Model = viewModel.Model,
                        Year = viewModel.Year,
                        RegistrationNumber = registrationNumber,
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
                        if (!string.IsNullOrEmpty(viewModel.RegistrationNumber))
                        {
                            existingCar = _context.Cars
                                .Where(c => c.RegistrationNumber == viewModel.RegistrationNumber)
                                .FirstOrDefault();
                        }
                        else if (!string.IsNullOrEmpty(viewModel.VIN))
                        {
                            existingCar = _context.Cars.FirstOrDefault(c => c.VIN == viewModel.VIN);
                        }
                        else
                        {
                            existingCar = _context.Cars
                                .FirstOrDefault(c => c.ClientId == client.Id 
                                    && c.Brand == viewModel.Brand 
                                    && c.Model == viewModel.Model 
                                    && c.Year == viewModel.Year);
                        }
                            
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

                // Изпращаме real-time известие към всички админи
                await _notificationService.NotifyAdmins(
                    $"Нова заявка от {client.Name} за {viewModel.ServiceType}",
                    "NewServiceRequest",
                    serviceRequest.Id
                );

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

                // Изпращаме имейл ако е посочен
                if (!string.IsNullOrEmpty(viewModel.Email))
                {
                    await _emailService.SendServiceRequestConfirmationAsync(
                        viewModel.Email,
                        client.Name,
                        serviceRequest.Id,
                        viewModel.ServiceType,
                        serviceRequest.CreatedOn
                    );
                    Console.WriteLine($"Confirmation email sent to {viewModel.Email}");
                }

                // Изпращаме имейл към админа за нова заявка
                var carInfo = !string.IsNullOrEmpty(viewModel.RegistrationNumber)
                    ? $"{viewModel.Brand} {viewModel.Model} ({viewModel.Year}) - {viewModel.RegistrationNumber}"
                    : $"{viewModel.Brand} {viewModel.Model} ({viewModel.Year})";
                await _emailService.SendAdminNotificationEmailAsync(
                    client.Name,
                    client.Phone,
                    viewModel.ServiceType,
                    carInfo,
                    serviceRequest.Id,
                    serviceRequest.CreatedOn
                );
                Console.WriteLine("Admin notification email sent");

                // Проверяваме дали клиентът беше логнат ПРЕДИ да подаде заявката
                // (проверяваме сесията ПРЕДИ създаването на клиента)
                var isLoggedIn = !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
                
                Console.WriteLine($"=== REDIRECT LOGIC ===");
                Console.WriteLine($"IsLoggedIn: {isLoggedIn}");
                Console.WriteLine($"Client IsGuest: {client.IsGuest}");
                
                // Ако клиентът е маркиран като гост ИЛИ не е логнат, го пращаме към Confirmation
                if (client.IsGuest || !isLoggedIn)
                {
                    // Гост заявка - пращаме към потвърдителна страница
                    TempData["SuccessMessage"] = "Вашата заявка е изпратена успешно! Ще се свържем с вас скоро.";
                    TempData["RequestId"] = serviceRequest.Id.ToString();
                    TempData["ClientEmail"] = viewModel.Email;
                    Console.WriteLine($"Redirecting to Confirmation page");
                    return RedirectToAction("Confirmation");
                }
                else
                {
                    // Логнат потребител - пращаме към неговите заявки
                    TempData["SuccessMessage"] = "Вашата заявка е изпратена успешно!";
                    Console.WriteLine($"Redirecting to MyRequests page");
                    return RedirectToAction("MyRequests");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"======== ERROR ========");
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine($"STACK TRACE: {ex.StackTrace}");
                
                var innerEx = ex.InnerException;
                var level = 1;
                while (innerEx != null)
                {
                    Console.WriteLine($"INNER EXCEPTION {level}: {innerEx.Message}");
                    Console.WriteLine($"INNER STACK TRACE {level}: {innerEx.StackTrace}");
                    innerEx = innerEx.InnerException;
                    level++;
                }
                Console.WriteLine($"======================");
                
                // Показваме по-детайлна грешка на потребителя
                var errorMessage = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message;
                TempData["ErrorMessage"] = $"Възникна грешка: {errorMessage}";
                
                // Ако сме от бърза форма (няма View за Hero), редиректваме към Home
                if (Request.Headers["Referer"].ToString().Contains("/Home") || 
                    Request.Headers["Referer"].ToString().Contains("/#"))
                {
                    return RedirectToAction("Index", "Home");
                }
                
                return View(viewModel);
            }
        }

        // Моите заявки
        public IActionResult MyRequests()
        {
            Console.WriteLine("=== MY REQUESTS DEBUG ===");
            var userIdString = HttpContext.Session.GetString("UserId");
            var userType = HttpContext.Session.GetString("UserType");
            
            Console.WriteLine($"UserId from session: {userIdString}");
            Console.WriteLine($"UserType from session: {userType}");

            if (string.IsNullOrEmpty(userIdString) || string.IsNullOrEmpty(userType) || !Guid.TryParse(userIdString, out var userId))
            {
                Console.WriteLine("No session found, redirecting to Login");
                return RedirectToAction("Login", "Account");
            }

            // Само клиенти могат да виждат своите заявки
            if (userType != "Client")
            {
                Console.WriteLine($"User type is {userType}, not Client");
                TempData["ErrorMessage"] = "Само клиенти имат достъп до заявките";
                return RedirectToAction("Index", "Home");
            }

            Console.WriteLine($"Fetching requests for client: {userId}");
            var requests = _serviceRequestService.GetClientRequests(userId);
            Console.WriteLine($"Found {requests.Count} requests");
            
            foreach (var req in requests)
            {
                Console.WriteLine($"Request: {req.Id}, Type: {req.ServiceTypeName}, Car: {req.CarInfo}, Created: {req.CreatedOn}");
            }
            
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

        // Потвърждение за гост заявка
        public IActionResult Confirmation()
        {
            // Keep() запазва данните в TempData за повторно използване
            ViewBag.RequestId = TempData.Peek("RequestId")?.ToString();
            ViewBag.ClientEmail = TempData.Peek("ClientEmail")?.ToString();
            
            return View();
        }
    }
}
