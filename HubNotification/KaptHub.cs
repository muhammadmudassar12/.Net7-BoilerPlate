
using Microsoft.AspNetCore.SignalR;

namespace HubNotification;
public class KaptHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
