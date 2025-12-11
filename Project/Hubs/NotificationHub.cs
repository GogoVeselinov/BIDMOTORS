using Microsoft.AspNetCore.SignalR;

namespace Project
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // За админ панела добавяме в групата Admins
            var httpContext = Context.GetHttpContext();
            var userRole = httpContext?.Session.GetString("UserRole");
            
            if (userRole == "Admin" || userRole == "Manager")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
                Console.WriteLine($"Admin connected to SignalR: {Context.ConnectionId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        // Метод за изпращане на известие за нова заявка
        public async Task NotifyNewRequest(string message)
        {
            await Clients.Group("Admins").SendAsync("ReceiveNotification", message);
        }
    }
}
