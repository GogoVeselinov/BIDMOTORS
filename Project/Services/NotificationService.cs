using Microsoft.AspNetCore.SignalR;
using Project.Data;
using Project.Models.Entities;

namespace Project.Services
{
    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hub;
        private readonly ApplicationDbContext _db;

        public NotificationService(IHubContext<NotificationHub> hub, ApplicationDbContext db)
        {
            _hub = hub;
            _db = db;
        }

        // Създаване на известие за клиент
        public async Task CreateNotificationForClient(Guid clientId, string message, string? type = null, Guid? relatedEntityId = null)
        {
            var notification = new Notification
            {
                ClientId = clientId,
                Message = message,
                Type = type,
                RelatedEntityId = relatedEntityId,
                IsRead = false,
                CreatedOn = DateTime.Now
            };

            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();

            // Изпращане на real-time известие
            await _hub.Clients.User(clientId.ToString()).SendAsync("ReceiveNotification", message);
        }

        // Създаване на известие за служител
        public async Task CreateNotificationForEmployee(Guid employeeId, string message, string? type = null, Guid? relatedEntityId = null)
        {
            var notification = new Notification
            {
                EmployeeId = employeeId,
                Message = message,
                Type = type,
                RelatedEntityId = relatedEntityId,
                IsRead = false,
                CreatedOn = DateTime.Now
            };

            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();

            // Изпращане на real-time известие
            await _hub.Clients.User(employeeId.ToString()).SendAsync("ReceiveNotification", message);
        }

        // Известие при промяна на статус на заявка
        public async Task NotifyServiceRequestStatusChange(Guid serviceRequestId, string newStatus)
        {
            var serviceRequest = _db.ServiceRequests
                .FirstOrDefault(sr => sr.Id == serviceRequestId);

            if (serviceRequest != null)
            {
                var message = $"Статусът на вашата заявка #{serviceRequest.Id.ToString().Substring(0, 8)} е променен на: {newStatus}";
                await CreateNotificationForClient(serviceRequest.ClientId, message, "ServiceRequest", serviceRequestId);
            }
        }

        // Известие при приключване на ремонт
        public async Task NotifyRepairCompleted(Guid repairId)
        {
            var repair = _db.Repairs
                .FirstOrDefault(r => r.Id == repairId);

            if (repair != null)
            {
                var message = $"Ремонтът на вашето превозно средство е завършен! Цена: {repair.Price:F2} лв.";
                await CreateNotificationForClient(repair.ClientId, message, "Repair", repairId);
            }
        }

        public async Task NotifyManagers(string message)
        {
            await _hub.Clients.Group("Managers").SendAsync("ReceiveNotification", message);
        }

        public async Task NotifyUser(string userId, string message)
        {
            await _hub.Clients.User(userId).SendAsync("ReceiveNotification", message);
        }

        public async Task NotifyAll(string message)
        {
            await _hub.Clients.All.SendAsync("ReceiveNotification", message);
        }

        // Вземане на брой непрочетени известия
        public int GetUnreadCount(Guid userId, string userType)
        {
            return userType == "Client"
                ? _db.Notifications.Count(n => n.ClientId == userId && !n.IsRead)
                : _db.Notifications.Count(n => n.EmployeeId == userId && !n.IsRead);
        }

        // Вземане на последни известия
        public List<Notification> GetRecentNotifications(Guid userId, string userType, int count = 5)
        {
            return userType == "Client"
                ? _db.Notifications.Where(n => n.ClientId == userId).OrderByDescending(n => n.CreatedOn).Take(count).ToList()
                : _db.Notifications.Where(n => n.EmployeeId == userId).OrderByDescending(n => n.CreatedOn).Take(count).ToList();
        }

        // Вземане на всички известия за потребител
        public List<Notification> GetAllNotifications(Guid userId, string userType)
        {
            return userType == "Client"
                ? _db.Notifications.Where(n => n.ClientId == userId).OrderByDescending(n => n.CreatedOn).ToList()
                : _db.Notifications.Where(n => n.EmployeeId == userId).OrderByDescending(n => n.CreatedOn).ToList();
        }

        // Маркиране на всички известия като прочетени
        public async Task<int> MarkAllAsRead(Guid userId, string userType)
        {
            var notifs = userType == "Client"
                ? _db.Notifications.Where(n => n.ClientId == userId && !n.IsRead).ToList()
                : _db.Notifications.Where(n => n.EmployeeId == userId && !n.IsRead).ToList();

            foreach (var notif in notifs)
            {
                notif.IsRead = true;
            }

            await _db.SaveChangesAsync();
            return notifs.Count;
        }

        // Създаване на тестово известие
        public async Task CreateTestNotification(Guid userId, string userType)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ClientId = userType == "Client" ? userId : null,
                EmployeeId = userType == "Employee" ? userId : null,
                Message = $"Тестово известие създадено на {DateTime.Now:dd.MM.yyyy HH:mm:ss}",
                Type = "Test",
                IsRead = false,
                CreatedOn = DateTime.Now
            };

            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();

            // Изпращане на real-time известие
            await _hub.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", notification.Message);
        }

        // Вземане на конкретно известие по ID
        public Notification? GetNotificationById(Guid id)
        {
            return _db.Notifications.FirstOrDefault(x => x.Id == id);
        }
    }
}