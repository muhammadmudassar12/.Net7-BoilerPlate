using EMS20.WebApi.Application.System.Notifications.Commands;
using EMS20.WebApi.Infrastructure.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PushNotifications.Firebase;
using System.Security.Claims;
using static Org.BouncyCastle.Math.EC.ECCurve;
using static System.Net.WebRequestMethods;

namespace EMS20.WebApi.Host.Controllers.System;

public class NotificationController : VersionedApiController
{
    private readonly HttpClient http;
    private readonly IConfiguration _configuration;

    public NotificationController(IConfiguration configuration)
    {
        http = new HttpClient();
        _configuration = configuration;
    }

    //[AllowAnonymous]
    //[HttpGet("notification.PostNotificationForAndroidAsync")]
    //public async Task PostNotificationForAndroid([FromQuery] string fcmReceiverToken)
    //{
    //    var settings = _configuration.GetSection(nameof(FirebaseSettings)).Get<FirebaseSettings>();
    //    var fcm = new FirebaseSender(settings, http);
    //    var payload = new
    //    {
    //        message = new
    //        {
    //            token = fcmReceiverToken,
    //            notification = new
    //            {
    //                title = "Test",
    //                body = "Test Body"
    //            }
    //        }
    //    };

    //    var response = await fcm.SendAsync(payload, settings);
    //}
    [HttpGet("notification.GetAllNotificationByUserAsync")]
    public async Task<ResultResponse<PaginationResponse<Application.System.Notifications.Dto.NotificationDto>>> GetAllNotificationByUserAsync([FromQuery] GetAllNotificationsByUserAsync request)
    {
        return await Mediator.Send(request);
    }

    [HttpDelete("notification.DeleteNotificationByIdAsync")]
    public async Task<ResultResponse<string>> DeleteNotificationByIdAsync([FromQuery]DeleteNotificationByIdAsync request)
    {
        request.UserId = new Guid(User.FindFirstValue("uid"));
        return await Mediator.Send(request);
    }
}
