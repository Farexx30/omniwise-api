using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Persistence.EntityConfigurations;

internal class LectureConfiguration : IEntityTypeConfiguration<Lecture>
{
    public void Configure(EntityTypeBuilder<Lecture> builder)
    {
        //One-to-many:
        builder.HasMany(l => l.Files)
            .WithOne()
            .HasForeignKey(fk => fk.LectureId);

        //Important: One-to-many relation with Course is already configured in CourseConfiguration.

        //Properties:
        builder.Property(l => l.Name)
            .HasMaxLength(256);
    }
}
