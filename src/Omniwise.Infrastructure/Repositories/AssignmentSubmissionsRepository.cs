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

    public async Task DeleteAsync(AssignmentSubmission assignmentSubmission)
    {
        dbContext.AssignmentSubmissions.Remove(assignmentSubmission);
        await dbContext.SaveChangesAsync();
    }

    public async Task<AssignmentSubmission?> GetByIdAsync(int assignmentSubmissionId)
    {
        var assignmentSubmission = await dbContext.AssignmentSubmissions
            .Include(asub => asub.Files)
            .Include(asub => asub.Comments)
                .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(asub => asub.Id == assignmentSubmissionId);

        if (assignmentSubmission is not null)
        {
            assignmentSubmission.Comments = [.. assignmentSubmission.Comments.OrderBy(c => c.SentDate)];
        }

        return assignmentSubmission;
    }

    public async Task<bool> IsAlreadySubmitted(int assignmentSubmissionId, string userId)
    {
        var isAlreadySubmitted = await dbContext.AssignmentSubmissions
            .AnyAsync(asub => asub.AssignmentId == assignmentSubmissionId
                      && asub.AuthorId == userId);

        return isAlreadySubmitted;
    }

    public async Task<bool> ExistsAsync(int assignmentSubmissionId)
    {
        var exists = await dbContext.AssignmentSubmissions
            .AnyAsync(asub => asub.Id == assignmentSubmissionId);

        return exists;
    }

    public Task SaveChangesAsync()
        => dbContext.SaveChangesAsync();
}
