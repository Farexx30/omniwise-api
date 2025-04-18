using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Persistence.EntityConfigurations;

internal class AssignmentSubmissionFileConfiguration : IEntityTypeConfiguration<AssignmentSubmissionFile>
{
    public void Configure(EntityTypeBuilder<AssignmentSubmissionFile> builder)
    {
        //Relations:
        //Important: One-to-many relation with Assignment is already configured in AssignmentSubmissionConfiguration.

        //Properties:
        builder.Property(asf => asf.Url)
            .HasMaxLength(2048);
    }
}
