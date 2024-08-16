using AutoMapper;
using EMS20.WebApi.Application.Identity.Users;
using EMS20.WebApi.Application.Resources.ResFile;
using EMS20.WebApi.Application.Services;
using EMS20.WebApi.Domain.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.Core.LookupManagement.Commands;
public class CreateLookupManagementRequestAsync : IRequest<ResultResponse<DefaultIdType>>
{
    public string? Name_Eng { get; set; }
    public string? Name_Ar { get; set; }
    public string? Decscription_Eng { get; set; }
    public string? Decscription_Ar { get; set; }
    public string? Arg1 { get; set; }
    public string? Arg2 { get; set; }
    public string? Arg3 { get; set; }
    public string? Arg4 { get; set; }
    public LookupTypes? LookupTypeId { get; set; }
    public int? SortOrder { get; set; }
    public Guid? ParentId { get; set; }
}
public class CreateLookupManagementRequestHandlerAsync : IRequestHandler<CreateLookupManagementRequestAsync, ResultResponse<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Domain.Core.LookupManagement> _repository;
    private readonly INotificationServices _notificationServices;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<langResApplication> _lang;
    private readonly IUserService _userService;

    public CreateLookupManagementRequestHandlerAsync(INotificationServices notificationServices,IUserService userService,IRepositoryWithEvents<Domain.Core.LookupManagement> repository, IMapper mapper, IStringLocalizer<langResApplication> lang)
    {
        _repository = repository;
        _mapper = mapper;
        _lang = lang;
        _userService = userService;
        _notificationServices = notificationServices;
    }
    public async Task<ResultResponse<DefaultIdType>> Handle(CreateLookupManagementRequestAsync request, CancellationToken cancellationToken)
    {
        var lookupManagement = _mapper.Map<Domain.Core.LookupManagement>(request);
        await _repository.AddAsync(lookupManagement, cancellationToken);
        var rolesForhub = new List<string> { "Admin",  "User" };
        var adminSupervisorUsers = await _userService.GetUsersByRolesAsync(rolesForhub);
        var usersTokenInformation = adminSupervisorUsers.Select(user => new UserTokenInformation { UserId = new Guid(user.Id) }).ToList();

        var notification = new NotificationDto
        {
            Title = $"Response submission for lookup Management",
            Message = $"Lookup entry is created successfully.",

        };
        await _notificationServices.SendNotificationToAll(notification, usersTokenInformation);

        return new ResultResponse<DefaultIdType>
        {
            Data = lookupManagement.Id,
            Success = true,
            Message = _lang["Success"].Value
        };
    }
}