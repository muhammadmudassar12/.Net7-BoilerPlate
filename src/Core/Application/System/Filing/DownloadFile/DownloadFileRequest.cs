using EMS20.WebApi.Application.System.Filing.Dto;

namespace EMS20.WebApi.Application.System.Filing.DownloadFile;
public class DownloadFileRequest : IRequest<ResultResponse<FileDto>>
{
    public string? FileName{ get; set; }
}

public class GetDowloadByNameAsyncSpec : Specification<Domain.System.File, Domain.System.File>, ISingleResultSpecification
{
    public GetDowloadByNameAsyncSpec(string fileName)
    {
        Query.Where(x => x.FileName == fileName);
    }
}
public class DownloadFileHandler : IRequestHandler<DownloadFileRequest, ResultResponse<FileDto>>
{
    private readonly IRepository<Domain.System.Settings> _settingsRepository;
    private readonly IRepository<Domain.System.File> _fileRepository;
    private readonly IFileStorageService _fileStorageService;

    public DownloadFileHandler(IRepository<Domain.System.Settings> settingsRepository, IRepository<Domain.System.File> fileRepository, IFileStorageService fileStorageService)
    {
        _settingsRepository = settingsRepository;
        _fileRepository = fileRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<ResultResponse<FileDto>> Handle(DownloadFileRequest request, CancellationToken cancellationToken)
    {
        var file = await _fileRepository.GetBySpecAsync(
            (ISpecification<Domain.System.File, Domain.System.File>)new GetDowloadByNameAsyncSpec(request.FileName), cancellationToken);

        var response = new FileDto
        {
            Data = file.FileData,
            Type = "application/octet-stream",
            FileName = file.FileName +file.Extension,
        };
        
        return new ResultResponse<FileDto> { Data = response, Success = true, Message = "Request Executed Successfully" };
    }
}
