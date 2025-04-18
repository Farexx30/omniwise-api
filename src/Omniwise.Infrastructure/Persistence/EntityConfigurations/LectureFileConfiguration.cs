using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Persistence.EntityConfigurations;

internal class LectureFileConfiguration : IEntityTypeConfiguration<LectureFile>
{
    public void Configure(EntityTypeBuilder<LectureFile> builder)
    {
        //Relations:
        //Important: One-to-many relation with Lecture is already configured in LectureConfiguration.

        //Properties:
        builder.Property(lf => lf.Url)
            .HasMaxLength(2048);
    }
}
