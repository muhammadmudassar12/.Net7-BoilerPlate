using Microsoft.AspNetCore.SignalR;
using PushNotifications.Interfaces;

namespace PushNotifications.SignalR;
public class EMSHub : Hub<IEMSHub>
{
    public async Task SendHubNotification(HubNotificationDto message)
    {
        await Clients.All.SendHubNotification(message);
    }
}
