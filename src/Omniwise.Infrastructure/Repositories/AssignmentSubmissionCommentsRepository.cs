using Microsoft.EntityFrameworkCore;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Repositories;

internal class AssignmentSubmissionCommentsRepository(OmniwiseDbContext dbContext) : IAssignmentSubmissionCommentsRepository
{
    public async Task<int> CreateAsync(AssignmentSubmissionComment comment)
    {
        dbContext.AssignmentSubmissionComments.Add(comment);
        await dbContext.SaveChangesAsync();

        return comment.Id;
    }

    public async Task<AssignmentSubmissionComment?> GetByIdAsync(int assignmentSubmissionCommentId)
    {
        var comment = await dbContext.AssignmentSubmissionComments
            .FirstOrDefaultAsync(asc => asc.Id == assignmentSubmissionCommentId);

        return comment;
    }

    public Task SaveChangesAsync()
        => dbContext.SaveChangesAsync();
}
