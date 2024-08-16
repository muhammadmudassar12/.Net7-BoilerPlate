

using EMS20.WebApi.Domain.System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using PushNotifications.Firebase;
using PushNotifications.Interfaces;
using PushNotifications.SignalR;

namespace EMS20.WebApi.Application.Services;

public interface INotificationServices : ITransientService
{
    List<FirebaseResponse> AggregateFirebaseResponses(FirebaseResponse[] responses);
    Task<FirebaseResponse> SendFirebaseNotificationAsync(NotificationDto notification, UserTokenInformation user, FirebaseSettings settings);
    Task<List<FirebaseResponse>> SendFirebaseNotifications(NotificationDto notification, List<UserTokenInformation> users);
    Task SendNotificationToAll(NotificationDto notification, List<UserTokenInformation> users);
    Task SendSignalRNotification(NotificationDto notification, List<UserTokenInformation> users);
    Task SendSignalRNotificationAsync(NotificationDto notification, UserTokenInformation user);
}

public class NotificationServices : INotificationServices
{
    private readonly IHubContext<EMSHub, IEMSHub> _hub;
    private readonly IFirebaseSender _firebaseSender;
    private readonly IConfiguration _configuration;
    private readonly IRepositoryWithEvents<Domain.System.Notifications> _repository;

    public NotificationServices(IHubContext<EMSHub, IEMSHub> hub, IFirebaseSender firebaseSender, IRepositoryWithEvents<Domain.System.Notifications> repository, IConfiguration configuration)
    {
        _hub = hub;
        _firebaseSender = firebaseSender;
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<List<FirebaseResponse>> SendFirebaseNotifications(NotificationDto notification, List<UserTokenInformation> users)
    {
        var settings = _configuration.GetSection(nameof(FirebaseSettings)).Get<FirebaseSettings>();
        var tasks = users.Select(user => SendFirebaseNotificationAsync(notification, user, settings));
        var responses = await Task.WhenAll(tasks);
        var aggregatedResponse = AggregateFirebaseResponses(responses);
        return aggregatedResponse;
    }
    public async Task<FirebaseResponse> SendFirebaseNotificationAsync(NotificationDto notification, UserTokenInformation user, FirebaseSettings settings)
    {
        var payload = new
        {
            message = new
            {
                token = user,
                notification = new
                {
                    title = notification.Title,
                    body = notification.Message
                }
            }
        };

        return await _firebaseSender.SendAsync(notification, settings);
    }
    public List<FirebaseResponse> AggregateFirebaseResponses(FirebaseResponse[] responses)
    {
        // Aggregate responses as needed
        // For example, you might want to check if any responses failed and handle accordingly
        return responses.ToList(); // Example, you might want to change this to fit your aggregation logic
    }
    public async Task SendSignalRNotification(NotificationDto notification, List<UserTokenInformation> users)
    {
        var tasks = users.Select(user => SendSignalRNotificationAsync(notification, user));
        await Task.WhenAll(tasks);
    }
    public async Task SendSignalRNotificationAsync(NotificationDto notification, UserTokenInformation user)
    {
        var hubDto = new HubNotificationDto
        {
            UserId = user.UserId,
            Title = notification.Title,
            Message = notification.Message,
        };

        var notifications = new Notifications
        {
            ReferenceTable = notification.ReferenceTable,
            ReferenceId = notification.ReferenceId,
            IsRead = false,
            ToUserId = user.UserId,
            Message = notification.Message,
            Title = notification.Title
        };

        await _repository.AddAsync(notifications);
        await _hub.Clients.All.SendHubNotification(hubDto);
    }
    public async Task SendNotificationToAll(NotificationDto notification, List<UserTokenInformation> users)
    {
        var notifications = users.ConvertAll(user => new Notifications
        {
            ReferenceTable = notification.ReferenceTable,
            ReferenceId = notification.ReferenceId,
            IsRead = false,
            ToUserId = user.UserId,
            Message = notification.Message,
            Title = notification.Title
        });

        SendFirebaseNotifications(notification, users);
        SendSignalRNotification(notification, users);
    }

}
