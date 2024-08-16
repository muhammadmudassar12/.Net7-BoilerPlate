using EMS20.WebApi.Application.Common.Interfaces;
using EMS20.WebApi.Application.Identity.Users;
using EMS20.WebApi.Application.System.Settings.Commands.CreateSettings;
using EMS20.WebApi.Application.System.Settings.Queries.GetSettings;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using PushNotifications.Interfaces;
using PushNotifications.SignalR;

namespace EMS20.WebApi.Host.Controllers.System;

public class SettingsController : VersionedApiController
{
    private readonly IMediator _mediator;
    private IHubContext<EMSHub, IEMSHub> _hub;
    public SettingsController(IMediator mediator, IHubContext<EMSHub, IEMSHub> hub) => (_mediator,_hub) = (mediator,hub);

    [HttpGet("settings.getSystemSettingsByGroupAsync")]
    [OpenApiOperation("Get System wide settings.", "")]
    public async Task<ResultResponse<List<SettingsDto>>> getSystemSettingsByGroupAsync([FromQuery] GetSettingsRequest request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(request, cancellationToken);

    }

    [HttpPost("settings.createSystemSettingsAsync")]
    [OpenApiOperation("Create System wide settings.", "")]
    public async Task<ActionResult> createSystemSettingsAsync([FromBody] CreateSettingsRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(request, cancellationToken));
    }

    [HttpPut("settings.updateSystemSettingsAsync")]
    [OpenApiOperation("update System wide settings.", "")]
    public async Task<ActionResult> UpdateSystemSettingsAsync([FromBody] UpdateSettingsRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(request, cancellationToken));
    }

    [HttpPost("settings.bulkInsertSystemSettingsAsync")]
    [OpenApiOperation("Create System wide settings.", "")]
    public async Task<ResultResponse<string>> bulkInsertSystemSettingsAsync([FromBody] List<CreateSettingsRequest> request, CancellationToken cancellationToken)
    {

        foreach (var item in request)
        {
            await _mediator.Send(request, cancellationToken);
        }

        return new ResultResponse<string> { Data = "Success", Message = "Request Executed Successfully", Success = true };
    }

    [HttpPost]
    [Route("sendHubNotification")]
    [AllowAnonymous]
    public string Get()
    {
        HubNotificationDto notification = new HubNotificationDto();

        notification.UserId = Guid.NewGuid();
        notification.Message = "Test hub Notification";
        notification.Priority= 1;

        _hub.Clients.All.SendHubNotification(notification);
        return "Offers sent successfully to all users!";
    }

}
