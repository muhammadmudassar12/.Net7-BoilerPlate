using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EMS20.WebApi.Domain.System;
public class OtpCode : AuditableEntity, IAggregateRoot
{
    public OtpCode() { }

    public OtpCode(string code, bool isUtilized, DefaultIdType userId, string origin, string sixDigitCode, DateTime expiryDate)
    {
        Code = code;
        IsUtilized = isUtilized;
        UserId = userId;
        Origin = origin;
        SixDigitCode = sixDigitCode;
        ExpiryDate = expiryDate;
    }

    public string Code { get; set; }
    public string SixDigitCode { get; set; }
    public bool IsUtilized { get; set; }
    public DateTime ExpiryDate { get; set; }
    public Guid UserId { get; set; }
    public string Origin { get; set; }

    public OtpCode Update(bool? newIsUtilized)
    {
        if (newIsUtilized.HasValue)
            IsUtilized = newIsUtilized.Value;

        return this;
    }

}
