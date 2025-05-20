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
        //Relations:
        //One-to-many:
        builder.HasMany(c => c.Lectures)
            .WithOne()
            .HasForeignKey(fk => fk.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Assignments)
            .WithOne(a => a.Course)
            .HasForeignKey(fk => fk.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        //Many-to-many:
        builder.HasMany(c => c.Members)
            .WithMany(u => u.EnrolledCourses)
            .UsingEntity<UserCourse>(
                l => l.HasOne<User>(uc => uc.User)
                    .WithMany(u => u.UserCourses)
                    .HasForeignKey(fk => fk.UserId)
                    .OnDelete(DeleteBehavior.Restrict),

                r => r.HasOne<Course>()
                    .WithMany()
                    .HasForeignKey(fk => fk.CourseId)
                    .OnDelete(DeleteBehavior.Cascade),

                j => j.HasKey(pk => new { pk.UserId, pk.CourseId })
            );

        //Properties:
        builder.Property(c => c.Name)
            .HasMaxLength(256);

        builder.Property(c => c.ImgUrl)
            .HasMaxLength(2048);
    }
}
