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
        //One-to-many:
        builder.HasMany<Notification>(u => u.Notifications)
            .WithOne()
            .HasForeignKey(fk => fk.UserId);

        //Important: Many-to-many relation with Course is already configured in CourseConfiguration.
    }
}
