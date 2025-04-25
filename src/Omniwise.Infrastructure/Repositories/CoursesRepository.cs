using Microsoft.EntityFrameworkCore;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Persistence;
using Omniwise.Application.Common.Interfaces;

namespace Omniwise.Infrastructure.Repositories;

internal class CoursesRepository(OmniwiseDbContext dbContext) : ICoursesRepository
{
    public async Task<Course?> GetCourseByIdAsync(int id)
    {
        var course = await dbContext.Courses.FirstOrDefaultAsync(c => c.Id == id);
        return course;
    }

    public async Task<IEnumerable<Course>> GetAllEnrolledCoursesAsync(string id)
    {
        var enrolledCourses = await dbContext.Courses
            .Include(c => c.Members)
            .Where(c => c.Members.Any(m => m.Id == id))
            .ToListAsync();

        return enrolledCourses;
    }

    public async Task<IEnumerable<Course>> GetAllOwnedCoursesAsync(string id)
    {
        var ownedCourses = await dbContext.Courses
            .Where(dbContext => dbContext.OwnerId == id)
            .ToListAsync();

        return ownedCourses;
    }
}
