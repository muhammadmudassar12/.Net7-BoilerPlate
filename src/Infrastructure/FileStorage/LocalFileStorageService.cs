using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using EMS20.WebApi.Domain.Common;
using EMS20.WebApi.Infrastructure.Common.Extensions;
using EMS20.WebApi.Application.Common.FileStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace EMS20.WebApi.Infrastructure.FileStorage;

public class LocalFileStorageService : IFileStorageService
{
    public async Task<string> UploadAsync<T>(FileUploadRequest? request, FileType supportedFileType, CancellationToken cancellationToken = default)
    where T : class
    {
        if (request == null || request.Data == null)
        {
            return string.Empty;
        }

        if (request.Extension is null || !supportedFileType.GetDescriptionList().Contains(request.Extension.ToLower()))
            throw new InvalidOperationException("File Format Not Supported.");
        if (request.Name is null)
            throw new InvalidOperationException("Name is required.");

        string base64Data = Regex.Match(request.Data, "data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;

        var streamData = new MemoryStream(Convert.FromBase64String(base64Data));
        if (streamData.Length > 0)
        {
            string folder = typeof(T).Name;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                folder = folder.Replace(@"\", "/");
            }

            string folderName = supportedFileType switch
            {
                FileType.Image => Path.Combine("Files", "Images", folder),
                _ => Path.Combine("Files", "Others", folder),
            };
            string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            Directory.CreateDirectory(pathToSave);

            string fileName = request.Name.Trim('"');
            fileName = RemoveSpecialCharacters(fileName);
            fileName = fileName.ReplaceWhitespace("-");
            fileName += request.Extension.Trim();
            string fullPath = Path.Combine(pathToSave, fileName);
            string dbPath = Path.Combine(folderName, fileName);
            if (File.Exists(dbPath))
            {
                dbPath = NextAvailableFilename(dbPath);
                fullPath = NextAvailableFilename(fullPath);
            }

            using var stream = new FileStream(fullPath, FileMode.Create);
            await streamData.CopyToAsync(stream, cancellationToken);
            return dbPath.Replace("\\", "/");
        }
        else
        {
            return string.Empty;
        }
    }

    public static string RemoveSpecialCharacters(string str)
    {
        return Regex.Replace(str, "[^a-zA-Z0-9_.]+", string.Empty, RegexOptions.Compiled);
    }

    public void Remove(string? path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private const string NumberPattern = "-{0}";

    private static string NextAvailableFilename(string path)
    {
        if (!File.Exists(path))
        {
            return path;
        }

        if (Path.HasExtension(path))
        {
            return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path), StringComparison.Ordinal), NumberPattern));
        }

        return GetNextFilename(path + NumberPattern);
    }

    private static string GetNextFilename(string pattern)
    {
        string tmp = string.Format(pattern, 1);

        if (!File.Exists(tmp))
        {
            return tmp;
        }

        int min = 1, max = 2;

        while (File.Exists(string.Format(pattern, max)))
        {
            min = max;
            max *= 2;
        }

        while (max != min + 1)
        {
            int pivot = (max + min) / 2;
            if (File.Exists(string.Format(pattern, pivot)))
            {
                min = pivot;
            }
            else
            {
                max = pivot;
            }
        }

        return string.Format(pattern, max);
    }

    public IFormFile ConvertByteArrayToFormFile(byte[] fileData, string fileName)
    {
        var stream = new MemoryStream(fileData);
        var formFile = new FormFile(stream, 0, fileData.Length, fileName, fileName);
        return formFile;
    }

    public byte[] ConvertFormFileToByteArray(IFormFile formFile)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            formFile.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }

    public async Task<int> UploadFileToServerAsync(IFormFile file, string serverDirectory, string fileName)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return 400;
            }

            if (!Directory.Exists(serverDirectory))
            {
                Directory.CreateDirectory(serverDirectory);
            }

            string fileExtension = Path.GetExtension(file.FileName);

            string uniqueFileName = $"{fileName}{fileExtension}";

            string filePath = Path.Combine(serverDirectory, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return 200;
        }
        catch (Exception ex)
        {
            return 500;
        }
    }


    public async Task<(byte[], string, string)> DownloadFile(string filePath)
    {
        try
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var _ContentType))
            {
                _ContentType = "application/octet-stream";
            }
            var _ReadAllBytesAsync = await File.ReadAllBytesAsync(filePath);
            return (_ReadAllBytesAsync, _ContentType, Path.GetFileName(filePath));
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}