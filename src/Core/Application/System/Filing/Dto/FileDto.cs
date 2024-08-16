using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS20.WebApi.Application.System.Filing.Dto;
public class FileDto
{
    public string? FileName { get; set; }
    public string? Type { get; set; }
    public byte[]? Data { get; set; }
}
