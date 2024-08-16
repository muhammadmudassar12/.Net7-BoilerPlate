using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS20.WebApi.Domain.Core;

namespace EMS20.WebApi.Infrastructure.Persistence.Configuration;
public class LookupManagementConfig : IEntityTypeConfiguration<LookupManagement>
{
    public void Configure(EntityTypeBuilder<LookupManagement> builder)
    {
        builder
            .ToTable("LookupManagement", SchemaNames.Core);

    }
}
