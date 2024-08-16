using EMS20.WebApi.Application.System.Filing.DownloadFile;
using EMS20.WebApi.Application.System.Filing.Dto;
using EMS20.WebApi.Application.System.Filing.UploadFile;
using EMS20.WebApi.Application.System.Settings.Queries.GetSettings;
using MediatR;
using MimeKit;
using System.Net;

namespace EMS20.WebApi.Host.Controllers.System;

public class FileController : VersionedApiController
{
    private readonly IMediator _mediator;
    public FileController(IMediator mediator) => _mediator = mediator;

    [HttpPost("file.uploadFileAsync")]
    [OpenApiOperation("Upload File.", "")]
    public async Task<ResultResponse<FileDto>> uploadFileAsync([FromQuery]UploadFileRequest? request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(request, cancellationToken);
    }

    [AllowAnonymous]
    [HttpGet("file.downloadFileasync")]
    [OpenApiOperation("Download File.", "")]
    public async Task<IActionResult> DownloadFileAsync([FromQuery] DownloadFileRequest? request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        var memoryStream = new MemoryStream(result.Data.Data);

        var safeFileName = WebUtility.UrlEncode(result.Data.FileName);
        Response.Headers["Content-Disposition"] = $"inline; filename*=UTF-8''{safeFileName}";
        return File(memoryStream, "application/octet-stream");
    }

}
