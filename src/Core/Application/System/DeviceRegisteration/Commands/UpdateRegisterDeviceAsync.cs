using AutoMapper;
using EMS20.WebApi.Application.Resources.ResFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.System.DeviceRegisteration.Commands;
public class UpdateRegisterDeviceRequestAsync : IRequest<ResultResponse<object>>
{
    public string DeviceId { get; set; }
    public string? DeviceName { get; set; }
    public string? Imei { get; set; }
    public string? DeviceType { get; set; }
    public DefaultIdType UserId { get; set; }
    public bool? IsLoggedIn { get; set; }
}

//public class GetDeviceRegisterationByIdSpec : Specification<Domain.System.DeviceRegisteration>, ISingleResultSpecification
//{
//    public GetDeviceRegisterationByIdSpec(DefaultIdType Id) =>
//        Query
//        .Where(b => b.UserId == Id);
//}

public class UpdateRegisterDeviceAsyncHandler : IRequestHandler<UpdateRegisterDeviceRequestAsync, ResultResponse<object>>
{
    private readonly IRepositoryWithEvents<Domain.System.DeviceRegisteration> _repository;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<langResApplication> _lang;

    public UpdateRegisterDeviceAsyncHandler(IRepositoryWithEvents<Domain.System.DeviceRegisteration> repository, IMapper mapper, IStringLocalizer<langResApplication> lang)
    {
        _repository = repository;
        _mapper = mapper;
        _lang = lang;
    }

    public async Task<ResultResponse<object>> Handle(UpdateRegisterDeviceRequestAsync request, CancellationToken cancellationToken)
    {
        var spec = (ISpecification<Domain.System.DeviceRegisteration, Domain.System.DeviceRegisteration>)new GetDeviceRegisterationByIdSpec(request.UserId);

        var DeviceRegisterationData = await _repository.GetBySpecAsync(spec);

        if(DeviceRegisterationData == null)
        {
            var insertDataDto = _mapper.Map<RegisterDeviceRequestAsync>(request);
            var insertData = _mapper.Map<Domain.System.DeviceRegisteration>(insertDataDto);
            await _repository.AddAsync(insertData, cancellationToken);
            return new ResultResponse<object>
            {
                Data = insertData,
                Success = true,
                Message = _lang["Success"].Value
            };
        }
        else
        {
            DeviceRegisterationData.DeviceId = request.DeviceId;
            DeviceRegisterationData.DeviceName = request.DeviceName;
            DeviceRegisterationData.Imei = request.DeviceType;
            DeviceRegisterationData.IsLoggedIn = request.IsLoggedIn;
            DeviceRegisterationData.DeviceType = request.DeviceType;

            await _repository.UpdateAsync(DeviceRegisterationData, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return new ResultResponse<object>
            {
                Data = DeviceRegisterationData,
                Success = true,
                Message = _lang["Success"].Value
            };
        }
    }
}