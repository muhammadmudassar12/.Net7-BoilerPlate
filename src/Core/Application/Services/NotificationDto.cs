namespace EMS20.WebApi.Application.Services;
public class NotificationDto
{
    public string? Message { get; set; }
    public string? Title { get; set; }
    public string? ReferenceTable { get; set; }
    public DefaultIdType? ReferenceId { get; set; }
}

public class UserTokenInformation
{
    public DefaultIdType? UserId { get; set; }
    public string? FcmToken { get; set; }
}

