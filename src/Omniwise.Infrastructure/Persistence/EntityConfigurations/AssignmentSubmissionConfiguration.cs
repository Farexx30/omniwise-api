using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Persistence.EntityConfigurations;

internal class AssignmentSubmissionConfiguration : IEntityTypeConfiguration<AssignmentSubmission>
{
    public void Configure(EntityTypeBuilder<AssignmentSubmission> builder)
    {
        //Relations:
        //One-to-many:
        builder.HasMany(asub => asub.Comments)
            .WithOne(asubc => asubc.AssignmentSubmission)
            .HasForeignKey(fk => fk.AssignmentSubmissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(asub => asub.Files)
            .WithOne()
            .HasForeignKey(fk => fk.AssignmentSubmissionId)
            .OnDelete(DeleteBehavior.Cascade);

        //Important: One-to-many relation with Assignment is already configured in AssignmentConfiguration.
    }
}
