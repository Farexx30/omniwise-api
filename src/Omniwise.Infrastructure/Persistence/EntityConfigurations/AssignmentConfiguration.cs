using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Persistence.EntityConfigurations;

internal class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        //Relations:
        //One-to-many:
        builder.HasMany(a => a.Submissions)
            .WithOne()
            .HasForeignKey(fk => fk.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Files)
            .WithOne()
            .HasForeignKey(fk => fk.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        //Important: One-to-many relation with Course is already configured in CourseConfiguration.

        //Properties:
        builder.Property(a => a.Name)
            .HasMaxLength(256);
    }
}
