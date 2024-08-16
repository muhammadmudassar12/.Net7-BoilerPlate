using System.Collections.ObjectModel;

namespace EMS20.WebApi.Shared.Authorization;

public static class FSHRoles
{
    public const string SuperAdmin = nameof(SuperAdmin);
    public const string Admin = nameof(Admin);
    public const string Trainer = nameof(Trainer);
    public const string Manager = nameof(Manager);
    public const string Supervisor = nameof(Supervisor);
    public const string Developer = nameof(Developer);
    public const string User = nameof(User);

    public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>(new[]
    {
        SuperAdmin, Admin, Manager, Supervisor, User, Trainer, Developer
    });

    public static bool IsDefault(string roleName) => DefaultRoles.Any(r => r == roleName);
}