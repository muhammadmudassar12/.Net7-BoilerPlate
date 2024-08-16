using EMS20.WebApi.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace EMS20.WebApi.Application.Common.FileStorage;

public interface IFileStorageService : ITransientService
{
    public Task<string> UploadAsync<T>(FileUploadRequest? request, FileType supportedFileType, CancellationToken cancellationToken = default)
    where T : class;

    public void Remove(string? path);

    public IFormFile ConvertByteArrayToFormFile(byte[]? fileData, string? fileName);

    public byte[] ConvertFormFileToByteArray(IFormFile? formFile);

    public Task<int> UploadFileToServerAsync(IFormFile? file,string server,string fileName);
    public Task<(byte[], string, string)> DownloadFile(string filePath);



}