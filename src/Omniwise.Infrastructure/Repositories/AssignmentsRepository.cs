using Microsoft.EntityFrameworkCore;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Application.Common.Types;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Repositories;

internal class AssignmentsRepository(OmniwiseDbContext dbContext) : IAssignmentsRepository
{
    public async Task<int> CreateAsync(Assignment assignment)
    {
        dbContext.Assignments.Add(assignment);
        await dbContext.SaveChangesAsync();

        return assignment.Id;
    }

    public async Task DeleteAsync(Assignment assignment)
    {
        dbContext.Assignments.Remove(assignment);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Assignment?> GetByIdAsync(int assignmentId, CurrentUser? currentUser = default, bool includeAssignmentSubmissions = false)
    {
        IQueryable<Assignment> query = dbContext.Assignments
            .Include(a => a.Files);

        if (includeAssignmentSubmissions)
        {
            if (currentUser is not null)
            {
                var isCurrentUserTeacher = currentUser.IsInRole(Roles.Teacher);

                query = query
                    .Include(a => a.Submissions
                        .Where(asub => isCurrentUserTeacher || asub.AuthorId == currentUser!.Id))
                        .ThenInclude(asub => asub.Author);
            }
        }

        var assignment = await query.FirstOrDefaultAsync(a => a.Id == assignmentId);

        return assignment;
    }

    public async Task<IEnumerable<int>> GetAllIdsByCourseIdAsync(int courseId)
    {
        var assignmentIds = await dbContext.Assignments
            .Where(a => a.CourseId == courseId)
            .Select(a => a.Id)
            .ToListAsync();

        return assignmentIds;
    }

    public async Task<IEnumerable<Assignment>> GetAllCourseAssignmentsAsync(int courseId)
    {
       var assignments = await dbContext.Assignments
            .AsNoTracking()
            .Where(c => c.CourseId == courseId)
            .ToListAsync();

        return assignments;
    }

    public async Task<float> GetMaxGradeAsync(int assignmentId)
    {
        var maxGrade = await dbContext.Assignments
            .Where(a => a.Id == assignmentId)
            .Select(a => a.MaxGrade)
            .FirstAsync(); //We can use FirstAsync here because we are sure that the assignment exists.

        return maxGrade;
    }

    public async Task<bool> ExistsAsync(int assignmentId)
    {
        var isExist = await dbContext.Assignments
            .AnyAsync(a => a.Id == assignmentId);

        return isExist;
    }


    public async Task<List<string>> GetAssigmentSubmissionAuthorIds(int assignmentId)
    {
        var authorIds = await dbContext.AssignmentSubmissions
            .Where(asub => asub.AssignmentId == assignmentId)
            .Select(asub => asub.AuthorId)
            .ToListAsync();

        return authorIds;
    }

    public Task SaveChangesAsync()
        => dbContext.SaveChangesAsync();
}
