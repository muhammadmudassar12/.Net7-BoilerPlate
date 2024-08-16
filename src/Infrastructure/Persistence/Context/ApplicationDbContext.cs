using Finbuckle.MultiTenant;
using EMS20.WebApi.Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using EMS20.WebApi.Application.Common.Events;
using EMS20.WebApi.Application.Common.Interfaces;
using EMS20.WebApi.Domain.System;

namespace EMS20.WebApi.Infrastructure.Persistence.Context;

public class ApplicationDbContext : BaseDbContext
{
    public ApplicationDbContext(ITenantInfo currentTenant, DbContextOptions<ApplicationDbContext> options, ICurrentUser currentUser, ISerializerService serializer, IOptions<DatabaseSettings> dbSettings, IEventPublisher events)
        : base(currentTenant, options, currentUser, serializer, dbSettings, events)
    {
    }
    public DbSet<Domain.System.Settings> Settings => Set<Domain.System.Settings>();
    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();

    public DbSet<Domain.System.File> Files => Set<Domain.System.File>();

    public DbSet<DeviceRegisteration> DeviceRegisteration => Set<DeviceRegisteration>();
    public DbSet<Domain.System.Notifications> Notifications => Set<Domain.System.Notifications>();
    public DbSet<Domain.Core.LookupManagement> LookupManagements => Set<Domain.Core.LookupManagement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(SchemaNames.Catalog);
    }
}