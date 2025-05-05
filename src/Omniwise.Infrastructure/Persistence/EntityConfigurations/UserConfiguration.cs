using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Persistence.EntityConfigurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        //Relations:
        //One-to-many:
        builder.HasMany(u => u.Notifications)
            .WithOne()
            .HasForeignKey(fk => fk.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.OwnedCourses)
            .WithOne()
            .HasForeignKey(fk => fk.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.AssignmentSubmissions)
            .WithOne()
            .HasForeignKey(fk => fk.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.AssignmentSubmissionComments)
            .WithOne(asc => asc.Author)
            .HasForeignKey(fk => fk.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        //Important: Many-to-many relation with Course is already configured in CourseConfiguration.

        //Properties:
        builder.Property(u => u.FirstName)
            .HasMaxLength(256);

        builder.Property(u => u.LastName)
            .HasMaxLength(256);
    }
}
