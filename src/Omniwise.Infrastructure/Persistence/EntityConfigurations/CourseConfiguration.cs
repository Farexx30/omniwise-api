using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Persistence.EntityConfigurations;

internal class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        //One-to-many:
        builder.HasMany(c => c.Lectures)
            .WithOne()
            .HasForeignKey(fk => fk.CourseId);

        builder.HasMany(c => c.Assignments)
            .WithOne()
            .HasForeignKey(fk => fk.CourseId);

        //Many-to-many:
        builder.HasMany(c => c.Members)
            .WithMany(u => u.Courses)
            .UsingEntity<UserCourse>(
                l => l.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(fk => fk.UserId),

                r => r.HasOne<Course>()
                    .WithMany()
                    .HasForeignKey(fk => fk.CourseId),

                j => j.HasKey(pk => new { pk.UserId, pk.CourseId })
            );
    }
}
