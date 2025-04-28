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

internal class AssignmentsRepository(OmniwiseDbContext dbContext) : IAssignmentsRepository
{
    public async Task<int> CreateAsync(Assignment assignment)
    {
        dbContext.Assignments.Add(assignment);
        await dbContext.SaveChangesAsync();

        return assignment.Id;
    }

    public async Task<Assignment?> GetByIdAsync(int assignmentId, int courseId)
    {
        var assignment = await dbContext.Assignments
            .FirstOrDefaultAsync(a => a.Id == assignmentId
                                && a.CourseId == courseId);

        return assignment;
    }

    public async Task<IEnumerable<Assignment>> GetAllCourseAssignmentsAsync(int courseId)
    {
       var assignments = await dbContext.Assignments
            .Where(c => c.CourseId == courseId)
            .ToListAsync();

        return assignments;
    }

    public Task SaveChangesAsync()
        => dbContext.SaveChangesAsync();
}
