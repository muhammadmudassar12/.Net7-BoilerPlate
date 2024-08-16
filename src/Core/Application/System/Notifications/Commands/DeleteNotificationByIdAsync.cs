using EMS20.WebApi.Application.System.Settings.Queries.GetSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.System.Notifications.Commands;
public class DeleteNotificationByIdAsync : IRequest<ResultResponse<string>>
{
    public DefaultIdType UserId { get; set; }
    public DefaultIdType NotificationId { get; set; }
}

public class GetNotificationByIdAsyncSpec : Specification<Domain.System.Notifications, Domain.System.Notifications>, ISingleResultSpecification
{
    public GetNotificationByIdAsyncSpec(DefaultIdType UserId, DefaultIdType NotificationId) =>

        Query
        .Where(x => x.Id == NotificationId);
}

public class DeleteNotificationByIdAsyncHandler : IRequestHandler<DeleteNotificationByIdAsync, ResultResponse<string>>
{
    private readonly IRepository<Domain.System.Notifications> _repository;
    private readonly IStringLocalizer _t;

    public DeleteNotificationByIdAsyncHandler(IRepository<Domain.System.Notifications> repository, IStringLocalizer<GetSettingsHandler> localizer) => (_repository, _t) = (repository, localizer);

    public async Task<ResultResponse<string>> Handle(DeleteNotificationByIdAsync request, CancellationToken cancellationToken)
    {
        var response = new ResultResponse<string>();
        var dto = await _repository.GetBySpecAsync(
        new GetNotificationByIdAsyncSpec(request.UserId, request.NotificationId), cancellationToken);

        if (dto == null)
        {
            throw new NotFoundException(_t["Notification doesn't exist."]);
        }

        await _repository.DeleteAsync(dto);

        response.Success = true;
        response.Message = _t["Request executed successfully."];
        return response;
    }

}


