using EMS20.WebApi.Application.Common.Interfaces;
using EMS20.WebApi.Shared.Events;

namespace EMS20.WebApi.Application.Common.Events;

public interface IEventPublisher : ITransientService
{
    Task PublishAsync(IEvent @event);
}