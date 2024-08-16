

namespace EMS20.WebApi.Domain.System;
public class File : AuditableEntity, IAggregateRoot
{
    public File() { }

    public File(string? fileName, string? extension, byte[]? fileData)
    {
        FileName = fileName;
        Extension = extension;
        FileData = fileData;
    }

    public string? FileName { get; set; }
    public string? Extension { get; set; }
    public byte[]? FileData { get; set; }
}
