using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.System.Notifications.Dto;
public class NotificationDto
{
    public DefaultIdType Id { get; set; }
    public string? Message { get; set; }
    public DefaultIdType? ToUserId { get; set; }
    public string? Title { get; set; }
    public bool? IsRead { get; set; }
    public DefaultIdType? ReferenceId { get; set; }
    public string? ReferenceTable { get; set; }
    public DateTime? CreatedOn { get; set; }
}
