using AutoMapper;
using EMS20.WebApi.Application.Resources.ResFile;

namespace EMS20.WebApi.Application.System.DeviceRegisteration.Commands;
public class RegisterDeviceRequestAsync : IRequest<ResultResponse<DefaultIdType>>
{
    public string DeviceId { get; set; }
    public string? DeviceName { get; set; }
    public string? Imei { get; set; }
    public string? DeviceType { get; set; }
    public DefaultIdType UserId { get; set; }
    public bool? IsLoggedIn { get; set; }
}

public class GetDeviceRegisterationByIdSpec : Specification<Domain.System.DeviceRegisteration, Domain.System.DeviceRegisteration>, ISingleResultSpecification
{
    public GetDeviceRegisterationByIdSpec(DefaultIdType Id) =>
        Query
        .Where(b => b.UserId == Id);
}

public class RegisterDeviceAsyncHandler : IRequestHandler<RegisterDeviceRequestAsync, ResultResponse<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Domain.System.DeviceRegisteration> _repository;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<langResApplication> _lang;

    public RegisterDeviceAsyncHandler(IRepositoryWithEvents<Domain.System.DeviceRegisteration> repository, IMapper mapper, IStringLocalizer<langResApplication> lang)
    {
        _repository = repository;
        _mapper = mapper;
        _lang = lang;
    }

    public async Task<ResultResponse<DefaultIdType>> Handle(RegisterDeviceRequestAsync request, CancellationToken cancellationToken)
    {
        var spec = (ISpecification<Domain.System.DeviceRegisteration, Domain.System.DeviceRegisteration>)new GetDeviceRegisterationByIdSpec(request.UserId);

        var DeviceRegisterationData = await _repository.GetBySpecAsync(spec);

        if (DeviceRegisterationData != null)
        {
            await _repository.DeleteAsync(DeviceRegisterationData);
        }

        var entity = _mapper.Map<Domain.System.DeviceRegisteration>(request);

        await _repository.AddAsync(entity, cancellationToken);

        return new ResultResponse<DefaultIdType>
        {
            Data = entity.Id,
            Success = true,
            Message = _lang["Success"].Value
        };
    }
}
