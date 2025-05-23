using Microsoft.EntityFrameworkCore;
using Omniwise.Application.AssignmentSubmissionComments.Dtos;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Domain.Constants;
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

    public async Task DeleteAsync(AssignmentSubmissionComment comment)
    {
        dbContext.AssignmentSubmissionComments.Remove(comment);
        await dbContext.SaveChangesAsync();
    }

    public async Task<AssignmentSubmissionComment?> GetByIdAsync(int assignmentSubmissionCommentId)
    {
        var comment = await dbContext.AssignmentSubmissionComments
            .FirstOrDefaultAsync(asc => asc.Id == assignmentSubmissionCommentId);

        return comment;
    }

    public async Task<AssignmentSubmissionCommentNotificationDto?> GetDetailsToCommentNotificationAsync(int assignmentSubmissionId)
    {
        var result = await dbContext.AssignmentSubmissions
             .Where(asub => asub.Id == assignmentSubmissionId)
             .Select(asub => new AssignmentSubmissionCommentNotificationDto
             {
                 CourseName = asub.Assignment.Course.Name,
                 CourseId = asub.Assignment.Course.Id,
                 AssignmentName = asub.Assignment.Name,
                 AssignmentSubmissionAuthorFirstName = asub.Author.FirstName,
                 AssignmentSubmissionAuthorLastName = asub.Author.LastName,
             })
             .FirstOrDefaultAsync();
     
        return result;
    }

    public Task SaveChangesAsync()
        => dbContext.SaveChangesAsync();
}
