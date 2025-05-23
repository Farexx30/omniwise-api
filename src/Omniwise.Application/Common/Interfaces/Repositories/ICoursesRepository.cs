using Omniwise.Domain.Entities;

namespace Omniwise.Application.Common.Interfaces.Repositories;

public interface ICoursesRepository
{
    Task<int> CreateAsync(Course course);
    Task DeleteAsync(Course course);
    Task<Course?> GetCourseByIdAsync(int id);
    Task<IEnumerable<Course>> GetAllEnrolledCoursesAsync(string id);
    Task<IEnumerable<Course>> GetAllOwnedCoursesAsync(string id);
    Task<IEnumerable<Course>> GetAvailableToEnrollCoursesMatchingAsync(string? searchPhrase, string id);
    Task<bool> ExistsAsync(int courseId);
    Task SaveChangesAsync();
}