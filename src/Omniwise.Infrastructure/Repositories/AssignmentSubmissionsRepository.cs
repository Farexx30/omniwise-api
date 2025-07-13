using AutoMapper.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Omniwise.Application.AssignmentSubmissionComments.Dtos;
using Omniwise.Application.AssignmentSubmissions.Dtos;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Application.CourseMembers.Dtos;
using Omniwise.Domain.Constants;
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

    public async Task DeleteByAuthorIdAsync(string authorId)
    {
        await dbContext.AssignmentSubmissions
            .Where(asub => asub.AuthorId == authorId)
            .ExecuteDeleteAsync();
    }

    public async Task<AssignmentSubmission?> GetByIdAsync(int assignmentSubmissionId, bool includeAuthor = false, bool includeFiles = false, bool includeAssignmentInfo = false, bool includeComments = false)
    {
        var mainQuery = dbContext.AssignmentSubmissions
            .AsQueryable();

        if (includeAuthor)
        {
            mainQuery = mainQuery
                .Include(asub => asub.Author);
        }

        if (includeFiles)
        {
            mainQuery = mainQuery
                .Include(asub => asub.Files);
        }

        if (includeAssignmentInfo)
        {
            mainQuery = mainQuery
                .Include(asub => asub.Assignment);
        }

        if (includeComments)
        {
            mainQuery = mainQuery
                .Include(asub => asub.Comments)
                    .ThenInclude(c => c.Author);
        }

        var assignmentSubmission = await mainQuery
            .FirstOrDefaultAsync(asub => asub.Id == assignmentSubmissionId);

        if (includeComments && assignmentSubmission is not null)
        {
            assignmentSubmission.Comments = [.. assignmentSubmission.Comments.OrderBy(c => c.SentDate)];
        }

        return assignmentSubmission;
    }

    public async Task<IEnumerable<int>> GetAllIdsByAssignmentIdAsync(int assignmentId)
    {
        var assignmentSubmissionIds = await dbContext.AssignmentSubmissions
            .Where(asub => asub.AssignmentId == assignmentId)
            .Select(asub => asub.Id)
            .ToListAsync();

        return assignmentSubmissionIds;
    }

    public async Task<IEnumerable<int>> GetAllIdsByAssignmentIdsAsync(IEnumerable<int> assignmentIds)
    {
        var assignmentSubmissionIds = await dbContext.AssignmentSubmissions
            .Where(asub => assignmentIds.Contains(asub.AssignmentId))
            .Select(asub => asub.Id)
            .ToListAsync();

        return assignmentSubmissionIds;
    }
    public async Task<IEnumerable<int>> GetAllIdsByAuthorIdAsync(string authorId)
    {
        var assignmentSubmissionIds = await dbContext.AssignmentSubmissions
            .Where(asub => asub.AuthorId == authorId)
            .Select(asub => asub.Id)
            .ToListAsync();

        return assignmentSubmissionIds;
    }

    public async Task<bool> IsAlreadySubmittedAsync(int assignmentSubmissionId, string userId)
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

    public async Task<AssignmentSubmissionNotificationDetailsDto?> GetRelatedAssignmentAndCourseNamesAsync(int assignmentSubmissionId)
    {
        var result = await dbContext.AssignmentSubmissions
            .Where(asub => asub.Id == assignmentSubmissionId)
            .Select(asub => new AssignmentSubmissionNotificationDetailsDto
            { 
                AssignmentName = asub.Assignment.Name, 
                CourseName = asub.Assignment.Course.Name 
            })
            .FirstOrDefaultAsync();

        return result;
    }

    public Task SaveChangesAsync()
        => dbContext.SaveChangesAsync();
}
