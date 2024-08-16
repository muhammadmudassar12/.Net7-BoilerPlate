using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using EMS20.WebApi.Domain.System;
using File = EMS20.WebApi.Domain.System.File;

namespace EMS20.WebApi.Infrastructure.Persistence.Configuration;
public class SettingConfig : IEntityTypeConfiguration<Domain.System.Settings>
{
    public void Configure(EntityTypeBuilder<Domain.System.Settings> builder)
    {
        builder
            .ToTable("Settings", SchemaNames.System);

    }
}
public class OtpCodeConfig : IEntityTypeConfiguration<OtpCode>
{
    public void Configure(EntityTypeBuilder<OtpCode> builder)
    {
        builder
            .ToTable("OtpCode", SchemaNames.System);

    }
}
public class FileConfig : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder
            .ToTable("File", SchemaNames.System)
            ;

    }
}

public class DeviceRegisterationConfig : IEntityTypeConfiguration<DeviceRegisteration>
{
    public void Configure(EntityTypeBuilder<DeviceRegisteration> builder)
    {
        builder.ToTable("DeviceRegistration", SchemaNames.System);

    }
}

public class NotificationConfig : IEntityTypeConfiguration<Domain.System.Notifications>
{
    public void Configure(EntityTypeBuilder<Domain.System.Notifications> builder)
    {
        builder.ToTable("Notification", SchemaNames.System);

    }
}
