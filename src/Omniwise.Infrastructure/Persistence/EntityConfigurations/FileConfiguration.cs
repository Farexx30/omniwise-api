using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Omniwise.Domain.Entities.File;

namespace Omniwise.Infrastructure.Persistence.EntityConfigurations;

internal class FileConfiguration : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        //Relations:
        //Important: One-to-many relation with is already configured in LectureConfiguration.

        //Properties:
        builder.Property(f => f.Url)
            .HasMaxLength(2048);
    }
}
