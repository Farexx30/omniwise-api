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

internal class AssignmentSubmissionsRepository(OmniwiseDbContext dbContext) : IAssignmentSubmissionsRepository
{
    public async Task<int> CreateAsync(AssignmentSubmission assignmentSubmission)
    {
        dbContext.AssignmentSubmissions.Add(assignmentSubmission);
        await dbContext.SaveChangesAsync();

        return assignmentSubmission.Id;
    }

    public async Task<bool> IsAlreadySubmitted(int assignmentId, string userId)
    {
        var isAlreadySubmitted = await dbContext.AssignmentSubmissions
            .AnyAsync(asub => asub.AssignmentId == assignmentId
                      && asub.AuthorId == userId);

        return isAlreadySubmitted;
    }
}
