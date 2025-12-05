using Microsoft.AspNetCore.SignalR;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var user = Context.User;

        if (user.IsInRole("Admin") || user.IsInRole("Manager"))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Managers");
        }

        await base.OnConnectedAsync();
    }
}
