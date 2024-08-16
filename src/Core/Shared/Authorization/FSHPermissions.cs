using System.Collections.ObjectModel;

namespace EMS20.WebApi.Shared.Authorization;

public static class FSHAction
{
    public const string View = nameof(View);
    public const string Search = nameof(Search);
    public const string Create = nameof(Create);
    public const string Update = nameof(Update);
    public const string Delete = nameof(Delete);
    public const string Export = nameof(Export);
    public const string Generate = nameof(Generate);
    public const string Clean = nameof(Clean);
    public const string UpgradeSubscription = nameof(UpgradeSubscription);
}

public static class FSHResource
{
    public const string Tenants = nameof(Tenants);
    public const string Dashboard = nameof(Dashboard);
    public const string Hangfire = nameof(Hangfire);
    public const string Users = nameof(Users);
    public const string UserRoles = nameof(UserRoles);
    public const string Roles = nameof(Roles);
    public const string RoleClaims = nameof(RoleClaims);
    public const string Settings = nameof(Settings);
    public const string LookupManagement = nameof(LookupManagement);
}

public static class FSHPermissions
{
    private static readonly FSHPermission[] _superAdmin = new FSHPermission[]
    {
        new("View Dashboard", FSHAction.View, FSHResource.Dashboard),
        new("View Hangfire", FSHAction.View, FSHResource.Hangfire),
        new("View Users", FSHAction.View, FSHResource.Users, IsDeveloper:true),
        new("Search Users", FSHAction.Search, FSHResource.Users,IsDeveloper:true),
        new("Create Users", FSHAction.Create, FSHResource.Users),
        new("Update Users", FSHAction.Update, FSHResource.Users),
        new("Delete Users", FSHAction.Delete, FSHResource.Users),
        new("Export Users", FSHAction.Export, FSHResource.Users),
        new("View UserRoles", FSHAction.View, FSHResource.UserRoles,IsDeveloper:true),
        new("Search UserRoles", FSHAction.Search, FSHResource.UserRoles),
        new("Create UserRoles", FSHAction.Create, FSHResource.UserRoles),
        new("Update UserRoles", FSHAction.Update, FSHResource.UserRoles),
        new("Delete UserRoles", FSHAction.Delete, FSHResource.UserRoles),
        new("View Roles", FSHAction.View, FSHResource.Roles,IsDeveloper:true),
        new("Create Roles", FSHAction.Create, FSHResource.Roles),
        new("Update Roles", FSHAction.Update, FSHResource.Roles),
        new("Delete Roles", FSHAction.Delete, FSHResource.Roles),
        new("View RoleClaims", FSHAction.View, FSHResource.RoleClaims),
        new("Search RoleClaims", FSHAction.Search, FSHResource.RoleClaims),
        new("Create RoleClaims", FSHAction.Create, FSHResource.RoleClaims),
        new("Update RoleClaims", FSHAction.Update, FSHResource.RoleClaims),
        new("Delete RoleClaims", FSHAction.Delete, FSHResource.RoleClaims),
        new("View RoleClaims", FSHAction.View, FSHResource.LookupManagement),
        new("Search RoleClaims", FSHAction.Search, FSHResource.LookupManagement),
        new("Create RoleClaims", FSHAction.Create, FSHResource.LookupManagement),
        new("Update RoleClaims", FSHAction.Update, FSHResource.LookupManagement),
        new("Delete RoleClaims", FSHAction.Delete, FSHResource.LookupManagement),
        new("Create Settings", FSHAction.View, FSHResource.Settings),
        new("Update Settings", FSHAction.Search, FSHResource.Settings),
        new("View Settings", FSHAction.Search, FSHResource.Settings,IsDeveloper:true),
        new("Search Settings", FSHAction.Search, FSHResource.Settings),
        new("Delete Settings", FSHAction.Search, FSHResource.Settings),
        new("View Tenants", FSHAction.View, FSHResource.Tenants),
        new("Create Tenants", FSHAction.Create, FSHResource.Tenants),
        new("Update Tenants", FSHAction.Update, FSHResource.Tenants),
        new("Upgrade Tenant Subscription", FSHAction.UpgradeSubscription, FSHResource.Tenants)
    };

    private static readonly FSHPermission[] _admin = new FSHPermission[]
   {
        new("View Dashboard", FSHAction.View, FSHResource.Dashboard),
        new("View Hangfire", FSHAction.View, FSHResource.Hangfire),
        new("View Users", FSHAction.View, FSHResource.Users, IsDeveloper:true),
        new("Search Users", FSHAction.Search, FSHResource.Users,IsDeveloper:true),
        new("Create Users", FSHAction.Create, FSHResource.Users),
        new("Update Users", FSHAction.Update, FSHResource.Users),
        new("Delete Users", FSHAction.Delete, FSHResource.Users),
        new("Export Users", FSHAction.Export, FSHResource.Users),
        new("View UserRoles", FSHAction.View, FSHResource.UserRoles,IsDeveloper:true),
        new("Search UserRoles", FSHAction.Search, FSHResource.UserRoles),
        new("Create UserRoles", FSHAction.Create, FSHResource.UserRoles),
        new("Update UserRoles", FSHAction.Update, FSHResource.UserRoles),
        new("Delete UserRoles", FSHAction.Delete, FSHResource.UserRoles),
        new("View Roles", FSHAction.View, FSHResource.Roles,IsDeveloper:true),
        new("Create Roles", FSHAction.Create, FSHResource.Roles),
        new("Update Roles", FSHAction.Update, FSHResource.Roles),
        new("Delete Roles", FSHAction.Delete, FSHResource.Roles),
        new("View RoleClaims", FSHAction.View, FSHResource.RoleClaims),
        new("Search RoleClaims", FSHAction.Search, FSHResource.RoleClaims),
        new("Create RoleClaims", FSHAction.Create, FSHResource.RoleClaims),
        new("Update RoleClaims", FSHAction.Update, FSHResource.RoleClaims),
        new("Delete RoleClaims", FSHAction.Delete, FSHResource.RoleClaims),
        new("View RoleClaims", FSHAction.View, FSHResource.LookupManagement),
        new("Search RoleClaims", FSHAction.Search, FSHResource.LookupManagement),
        new("Create RoleClaims", FSHAction.Create, FSHResource.LookupManagement),
        new("Update RoleClaims", FSHAction.Update, FSHResource.LookupManagement),
        new("Delete RoleClaims", FSHAction.Delete, FSHResource.LookupManagement),
        new("Create Settings", FSHAction.View, FSHResource.Settings),
        new("Update Settings", FSHAction.Search, FSHResource.Settings),
        new("View Settings", FSHAction.Search, FSHResource.Settings,IsDeveloper:true),
        new("Search Settings", FSHAction.Search, FSHResource.Settings),
        new("Delete Settings", FSHAction.Search, FSHResource.Settings),
        new("View Tenants", FSHAction.View, FSHResource.Tenants),
        new("Create Tenants", FSHAction.Create, FSHResource.Tenants),
        new("Update Tenants", FSHAction.Update, FSHResource.Tenants),
        new("Upgrade Tenant Subscription", FSHAction.UpgradeSubscription, FSHResource.Tenants)
   };

    private static readonly FSHPermission[] _developer = new FSHPermission[]
   {
         new("View Dashboard", FSHAction.View, FSHResource.Dashboard),
        new("View Hangfire", FSHAction.View, FSHResource.Hangfire),
        new("View Users", FSHAction.View, FSHResource.Users, IsDeveloper:true),
        new("Search Users", FSHAction.Search, FSHResource.Users,IsDeveloper:true),
        new("View UserRoles", FSHAction.View, FSHResource.UserRoles,IsDeveloper:true),
        new("Search UserRoles", FSHAction.Search, FSHResource.UserRoles),
        new("View Roles", FSHAction.View, FSHResource.Roles,IsDeveloper:true),
        new("View RoleClaims", FSHAction.View, FSHResource.RoleClaims),
        new("Search RoleClaims", FSHAction.Search, FSHResource.RoleClaims),
        new("View LookupManagement", FSHAction.View, FSHResource.LookupManagement),
        new("Search LookupManagement", FSHAction.Search, FSHResource.LookupManagement),
        new("View Settings", FSHAction.Search, FSHResource.Settings,IsDeveloper:true),
        new("Search Settings", FSHAction.Search, FSHResource.Settings),
        new("View Tenants", FSHAction.View, FSHResource.Tenants)
   };

    private static readonly FSHPermission[] _manager = new FSHPermission[]
   {
        new("View Dashboard", FSHAction.View, FSHResource.Dashboard),
        new("View Hangfire", FSHAction.View, FSHResource.Hangfire),
        new("View Users", FSHAction.View, FSHResource.Users, IsDeveloper:true),
        new("Search Users", FSHAction.Search, FSHResource.Users,IsDeveloper:true),
        new("View UserRoles", FSHAction.View, FSHResource.UserRoles,IsDeveloper:true),
        new("Search UserRoles", FSHAction.Search, FSHResource.UserRoles),
        new("View Roles", FSHAction.View, FSHResource.Roles,IsDeveloper:true),
        new("View RoleClaims", FSHAction.View, FSHResource.RoleClaims),
        new("Search RoleClaims", FSHAction.Search, FSHResource.RoleClaims),
        new("View LookupManagement", FSHAction.View, FSHResource.LookupManagement),
        new("Search LookupManagement", FSHAction.Search, FSHResource.LookupManagement),
        new("View Settings", FSHAction.Search, FSHResource.Settings,IsDeveloper:true),
        new("Search Settings", FSHAction.Search, FSHResource.Settings),
        new("View Tenants", FSHAction.View, FSHResource.Tenants)
   };

    private static readonly FSHPermission[] _trainer = new FSHPermission[]
   {
         new("View Dashboard", FSHAction.View, FSHResource.Dashboard),
        new("View Hangfire", FSHAction.View, FSHResource.Hangfire),
        new("View Users", FSHAction.View, FSHResource.Users, IsDeveloper:true),
        new("Search Users", FSHAction.Search, FSHResource.Users,IsDeveloper:true),
        new("View UserRoles", FSHAction.View, FSHResource.UserRoles,IsDeveloper:true),
        new("Search UserRoles", FSHAction.Search, FSHResource.UserRoles),
        new("View Roles", FSHAction.View, FSHResource.Roles,IsDeveloper:true),
        new("View RoleClaims", FSHAction.View, FSHResource.RoleClaims),
        new("Search RoleClaims", FSHAction.Search, FSHResource.RoleClaims),
        new("View LookupManagement", FSHAction.View, FSHResource.LookupManagement),
        new("Search LookupManagement", FSHAction.Search, FSHResource.LookupManagement),
        new("View Settings", FSHAction.Search, FSHResource.Settings,IsDeveloper:true),
        new("Search Settings", FSHAction.Search, FSHResource.Settings),
        new("View Tenants", FSHAction.View, FSHResource.Tenants)
   };

    private static readonly FSHPermission[] _supervisor = new FSHPermission[]
   {
        new("View Dashboard", FSHAction.View, FSHResource.Dashboard),
        new("View Hangfire", FSHAction.View, FSHResource.Hangfire),
        new("View Users", FSHAction.View, FSHResource.Users, IsDeveloper:true),
        new("Search Users", FSHAction.Search, FSHResource.Users,IsDeveloper:true),
        new("View UserRoles", FSHAction.View, FSHResource.UserRoles,IsDeveloper:true),
        new("Search UserRoles", FSHAction.Search, FSHResource.UserRoles),
        new("View Roles", FSHAction.View, FSHResource.Roles,IsDeveloper:true),
        new("View RoleClaims", FSHAction.View, FSHResource.RoleClaims),
        new("Search RoleClaims", FSHAction.Search, FSHResource.RoleClaims),
        new("View LookupManagement", FSHAction.View, FSHResource.LookupManagement),
        new("Search LookupManagement", FSHAction.Search, FSHResource.LookupManagement),
        new("View Settings", FSHAction.Search, FSHResource.Settings,IsDeveloper:true),
        new("Search Settings", FSHAction.Search, FSHResource.Settings),
        new("View Tenants", FSHAction.View, FSHResource.Tenants)
   };

    private static readonly FSHPermission[] _user = new FSHPermission[]
   {
        new("View Dashboard", FSHAction.View, FSHResource.Dashboard),
        new("View Hangfire", FSHAction.View, FSHResource.Hangfire),
        new("View Users", FSHAction.View, FSHResource.Users, IsDeveloper:true),
        new("Search Users", FSHAction.Search, FSHResource.Users,IsDeveloper:true),
        new("View UserRoles", FSHAction.View, FSHResource.UserRoles,IsDeveloper:true),
        new("Search UserRoles", FSHAction.Search, FSHResource.UserRoles),
        new("View Roles", FSHAction.View, FSHResource.Roles,IsDeveloper:true),
        new("View RoleClaims", FSHAction.View, FSHResource.RoleClaims),
        new("Search RoleClaims", FSHAction.Search, FSHResource.RoleClaims),
        new("View LookupManagement", FSHAction.View, FSHResource.LookupManagement),
        new("Search LookupManagement", FSHAction.Search, FSHResource.LookupManagement),
        new("View Settings", FSHAction.Search, FSHResource.Settings,IsDeveloper:true),
        new("Search Settings", FSHAction.Search, FSHResource.Settings),
        new("View Tenants", FSHAction.View, FSHResource.Tenants)
   };
    public static IReadOnlyList<FSHPermission> SuperAdmin { get; } = new ReadOnlyCollection<FSHPermission>(_superAdmin.Where(p => p.IsSuperAdmin).ToArray());
    public static IReadOnlyList<FSHPermission> Admin { get; } = new ReadOnlyCollection<FSHPermission>(_admin.Where(p => !p.IsAdmin).ToArray());
    public static IReadOnlyList<FSHPermission> Developer { get; } = new ReadOnlyCollection<FSHPermission>(_developer.Where(p => !p.IsDeveloper).ToArray());
    public static IReadOnlyList<FSHPermission> Manager { get; } = new ReadOnlyCollection<FSHPermission>(_manager.Where(p => !p.IsManager).ToArray());
    public static IReadOnlyList<FSHPermission> Trainer { get; } = new ReadOnlyCollection<FSHPermission>(_trainer.Where(p => !p.IsTrainer).ToArray());
    public static IReadOnlyList<FSHPermission> Supervisor { get; } = new ReadOnlyCollection<FSHPermission>(_supervisor.Where(p => !p.IsSupervisor).ToArray());
    public static IReadOnlyList<FSHPermission> User { get; } = new ReadOnlyCollection<FSHPermission>(_user.Where(p => !p.IsUser).ToArray());
}

public record FSHPermission(string Description, string Action, string Resource,
    bool IsSuperAdmin = false,
    bool IsAdmin = false,
    bool IsDeveloper = false,
    bool IsManager = false,
    bool IsTrainer = false,
    bool IsSupervisor = false,
    bool IsUser = false
    )
{
    public string Name => NameFor(Action, Resource);
    public static string NameFor(string action, string resource) => $"Permissions.{resource}.{action}";
}
