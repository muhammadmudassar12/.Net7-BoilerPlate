
using AutoMapper;
using EMS20.WebApi.Domain.Core;

namespace EMS20.WebApi.Application.MappingProfiles;
public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<LookupManagement, Core.LookupManagement.Commands.CreateLookupManagementRequestAsync>().ReverseMap();
        CreateMap<LookupManagement, Core.LookupManagement.Commands.UpdateLookupManagementRequestAsync>().ReverseMap();
        CreateMap<LookupManagement, Core.LookupManagement.Commands.DeleteLookupManagementRequestAsync>().ReverseMap();
        CreateMap<LookupManagement, Core.LookupManagement.ResponseDto.LookupManagementDto>().ReverseMap();
    }
}
