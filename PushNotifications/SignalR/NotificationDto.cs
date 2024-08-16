using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushNotifications.SignalR;
public class NotificationDto
{
    public Guid UserId { get; set; }
    public int Priority { get; set; }
    public string? Message { get; set; }
    public string? MessageAR { get; set; }
}
