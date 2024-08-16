using AutoMapper;
using EMS20.WebApi.Application.Common.Specification;
using EMS20.WebApi.Application.System.Notifications.Dto;
using EMS20.WebApi.Application.System.Settings.Queries.GetSettings;
using EMS20.WebApi.Application.System.Settings.TimeZoneSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.System.Notifications.Commands;
public class GetAllNotificationsByUserAsync : ListRequest, IRequest<ResultResponse<PaginationResponse<NotificationDto>>>
{
    public Search? Search { get; set; }
}

public class GetAllNotificationByUserAsyncSpec : Specification<Domain.System.Notifications, NotificationDto>, ISingleResultSpecification
{
    public GetAllNotificationByUserAsyncSpec(PaginationFilter filter, Search search) =>
        Query
            .AdvancedSearch(search)
            .PaginateBy(filter)
            .Include(x => x.ToUser)
            .OrderByDescending(x => x.CreatedOn);
}

public class GetAllNotificationsByUserAsyncHandler : IRequestHandler<GetAllNotificationsByUserAsync, ResultResponse<PaginationResponse<NotificationDto>>>
{
    private readonly IRepository<Domain.System.Notifications> _notificationRepository;
    private readonly ITimeZoneService _timeZoneService;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer _t;

    public GetAllNotificationsByUserAsyncHandler(IRepository<Domain.System.Notifications> notificationRepository,  ITimeZoneService timeZoneService, IStringLocalizer<GetSettingsHandler> localizer,  IMapper mapper)
    {
        _notificationRepository = notificationRepository;
        _timeZoneService = timeZoneService;
        _mapper = mapper;
        _t = localizer;
    }

    public async Task<ResultResponse<PaginationResponse<NotificationDto>>> Handle(GetAllNotificationsByUserAsync request, CancellationToken cancellationToken)
    {
        var response = new ResultResponse<PaginationResponse<NotificationDto>>();
        var offset = await _timeZoneService.GetTimeZoneOffsetAsync(cancellationToken);
        var notifications = await _notificationRepository.ListAsync(
            new GetAllNotificationByUserAsyncSpec(new PaginationFilter { PageNumber = request.PageNumber, PageSize = request.PageSize }, request.Search), cancellationToken);

        if (notifications == null || !notifications.Any())
        {
            response.Success = false;
            response.Message = _t["Notifications doesn't exist."];
            return response;
        }

        var notificationDto = notifications.Select(n => 
        {
            var mappedNotification = _mapper.Map<NotificationDto>(n);
            mappedNotification.CreatedOn = n.CreatedOn.HasValue ? n.CreatedOn.Value.AddMinutes(offset) : (DateTime?)null;
            return mappedNotification;
        }).ToList();

        var result = new PaginationResponse<NotificationDto>(notificationDto, await _notificationRepository.CountAsync(), request.PageNumber, request.PageSize);

        response.Success = true;
        response.Message = _t["Request executed successfully."];
        response.Data = result;
        return response;
    }
}
