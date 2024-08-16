using EMS20.WebApi.Application.System.Filing.Dto;
using Microsoft.AspNetCore.Http;

namespace EMS20.WebApi.Application.System.Filing.UploadFile;
public class UploadFileRequest : IRequest<ResultResponse<FileDto>>
{
    public IFormFile? File { get; set; }
}

public class GetSettingsByNameAsyncSpec : Specification<Domain.System.Settings, Domain.System.Settings>, ISingleResultSpecification
{
    public GetSettingsByNameAsyncSpec(string Name, string Group) =>
        Query
        .Where(x => x.Name == Name && x.Group == Group);
}

public class UploadFileHandler : IRequestHandler<UploadFileRequest, ResultResponse<FileDto>>
{
    private readonly IRepository<Domain.System.File> _fileRepository;
    private readonly IFileStorageService _fileStorageService;

    public UploadFileHandler(IRepository<Domain.System.File> fileRepository, IFileStorageService fileStorageService)
    {
        _fileRepository = fileRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<ResultResponse<FileDto>> Handle(UploadFileRequest request, CancellationToken cancellationToken)
    {
        var fileExtension = Path.GetExtension(request.File.FileName);
        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        var fileData = _fileStorageService.ConvertFormFileToByteArray(request.File);
        var entity = new Domain.System.File(fileName.ToString(), fileExtension, fileData);
        var dto = new FileDto
        {
            Data = fileData,
            FileName = fileName,
        };
        await _fileRepository.AddAsync(entity);

        return new ResultResponse<FileDto> { Data = dto, Success = true, Message = "Request Executed Successfully" };
    }
}
