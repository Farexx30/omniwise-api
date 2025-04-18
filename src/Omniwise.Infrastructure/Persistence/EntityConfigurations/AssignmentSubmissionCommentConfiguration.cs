using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Persistence.EntityConfigurations;

internal class AssignmentSubmissionCommentConfiguration : IEntityTypeConfiguration<AssignmentSubmissionComment>
{
    public void Configure(EntityTypeBuilder<AssignmentSubmissionComment> builder)
    {
        //Relations:
        //Important: One-to-many relation with AssignmentSubmission is already configured in AssignmentSubmissionConfiguration.

        //Properties:
        builder.Property(asc => asc.Content)
            .HasMaxLength(2000);
    }
}
