using PushNotifications.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushNotifications.Interfaces;
public interface IEMSHub
{
    Task SendHubNotification(HubNotificationDto message);
}
