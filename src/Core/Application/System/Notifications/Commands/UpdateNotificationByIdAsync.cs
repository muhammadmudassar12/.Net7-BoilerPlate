using EMS20.WebApi.Application.System.Settings.Queries.GetSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.System.Notifications.Commands;
public class UpdateNotificationByIdAsync : IRequest<ResultResponse<string>>
{
    public DefaultIdType UserId { get; set; }
    public DefaultIdType NotificationId { get; set; }
}


public class UpdateNotificationByIdAsyncHandler : IRequestHandler<UpdateNotificationByIdAsync, ResultResponse<string>>
{
    private readonly IRepository<Domain.System.Notifications> _repository;
    private readonly IStringLocalizer _t;

    public UpdateNotificationByIdAsyncHandler(IRepository<Domain.System.Notifications> repository, IStringLocalizer<GetSettingsHandler> localizer) => (_repository, _t) = (repository, localizer);

    public async Task<ResultResponse<string>> Handle(UpdateNotificationByIdAsync request, CancellationToken cancellationToken)
    {
        var response = new ResultResponse<string>();
        var dto = await _repository.GetByIdAsync(request.NotificationId);



        if (dto == null)
        {
            response.Success = false;
            response.Message = _t["Notification doesn't exist."];
            return response;
        }

        dto.IsRead = true;

        await _repository.UpdateAsync(dto, cancellationToken);

        response.Success = true;
        response.Message = _t["Request executed Successfully."];
        response.Data = null;
        return response;
    }

}

