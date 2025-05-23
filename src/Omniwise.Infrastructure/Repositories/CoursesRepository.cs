using Microsoft.EntityFrameworkCore;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Persistence;
using Omniwise.Application.Common.Interfaces.Repositories;

namespace Omniwise.Infrastructure.Repositories;

internal class CoursesRepository(OmniwiseDbContext dbContext) : ICoursesRepository
{
    public async Task<int> CreateAsync(Course course)
    {
        dbContext.Courses.Add(course);
        await dbContext.SaveChangesAsync();

        return course.Id;
    }

    public async Task DeleteAsync(Course course)
    {
        dbContext.Courses.Remove(course);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Course?> GetCourseByIdAsync(int id)
    {
        var course = await dbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == id);

        return course;
    }

    public async Task<IEnumerable<Course>> GetAllEnrolledCoursesAsync(string id)
    {
        var enrolledCourses = await dbContext.UserCourses
            .AsNoTracking()
            .Where(uc => uc.UserId == id)
            .Select(uc => uc.Course)
            .ToListAsync();

        return enrolledCourses;
    }

    public async Task<IEnumerable<Course>> GetAllOwnedCoursesAsync(string id)
    {
        var ownedCourses = await dbContext.Courses
            .AsNoTracking()
            .Where(c => c.OwnerId == id)
            .ToListAsync();

        return ownedCourses;
    }

    public async Task<IEnumerable<Course>> GetAvailableToEnrollCoursesMatchingAsync(string? searchPhrase, string id)
    {
        var availableCourses = await dbContext.Courses
            .AsNoTracking()
            .Where(c => !c.Members.Any(m => m.Id == id))
            .Where(c => string.IsNullOrWhiteSpace(searchPhrase)
                   || c.Name.Contains(searchPhrase.Trim()))
            .ToListAsync();

        return availableCourses;
    }

    public async Task<bool> ExistsAsync(int courseId)
    {
        var exists = await dbContext.Courses
            .AnyAsync(c => c.Id == courseId);

        return exists;
    }

    public Task SaveChangesAsync()
        => dbContext.SaveChangesAsync();
}