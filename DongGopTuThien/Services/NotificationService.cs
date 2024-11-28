using Microsoft.AspNetCore.SignalR;

namespace DongGopTuThien.Services;

public class NotificationService : Hub
{
    // Method to send a message to all connected clients
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}