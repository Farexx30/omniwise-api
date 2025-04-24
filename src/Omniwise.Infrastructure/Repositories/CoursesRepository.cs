using Microsoft.EntityFrameworkCore;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Persistence;
using Omniwise.Domain.Repositories;

namespace Omniwise.Infrastructure.Repositories;

internal class CoursesRepository(OmniwiseDbContext dbContext) : ICoursesRepository
{
    public async Task<Course?> GetCourseByIdAsync(int id)
    {
        var course = await dbContext.Courses.FirstOrDefaultAsync(c => c.Id == id);
        return course;
    }
}
