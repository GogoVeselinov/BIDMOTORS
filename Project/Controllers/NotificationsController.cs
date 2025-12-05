using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Project.Services;

namespace Project.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly NotificationService _notificationService;

        public NotificationsController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public IActionResult All()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            var userType = HttpContext.Session.GetString("UserType");

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var notifs = _notificationService.GetAllNotifications(userId, userType);
            return View(notifs);
        }

        [HttpGet]
        public async Task<IActionResult> CreateTest()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            var userType = HttpContext.Session.GetString("UserType");

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            await _notificationService.CreateTestNotification(userId, userType);
            return RedirectToAction("All");
        }

        [HttpGet]
        public JsonResult GetUnreadCount()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            var userType = HttpContext.Session.GetString("UserType");

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Json(new { count = 0 });
            }

            int count = _notificationService.GetUnreadCount(userId, userType);
            return Json(new { count });
        }

        [HttpGet]
        public JsonResult GetRecent()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            var userType = HttpContext.Session.GetString("UserType");

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Json(new { notifications = new List<object>() });
            }

            var notifs = _notificationService.GetRecentNotifications(userId, userType);
            var result = notifs.Select(n => new
            {
                id = n.Id,
                message = n.Message,
                isRead = n.IsRead,
                createdOn = n.CreatedOn.ToString("dd.MM.yyyy HH:mm"),
                type = n.Type
            });

            return Json(new { notifications = result });
        }

        public IActionResult View(Guid id)
        {
            var notif = _notificationService.GetNotificationById(id);

            if (notif == null)
                return NotFound();

            return PartialView("_NotificationDetails", notif);
        }

        [HttpPost]
        public async Task<JsonResult> MarkAsRead()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            var userType = HttpContext.Session.GetString("UserType");

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Json(new { success = false });
            }

            int count = await _notificationService.MarkAllAsRead(userId, userType);
            return Json(new { success = true, count });
        }
    }

}