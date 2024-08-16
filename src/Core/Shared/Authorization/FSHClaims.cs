namespace EMS20.WebApi.Shared.Authorization;

public static class FSHClaims
{
    public const string Tenant = "tenant";
    public const string Fullname = "fullName";
    public const string Permission = "permission";
    public const string ImageUrl = "image_url";
    public const string IpAddress = "ipAddress";
    public const string Expiration = "exp";
    public static readonly List<string> Roles = new List<string>();
    public static readonly List<string> Permissions = new List<string>();
}