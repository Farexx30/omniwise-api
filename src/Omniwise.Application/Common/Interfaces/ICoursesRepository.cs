using Omniwise.Domain.Entities;

namespace Omniwise.Application.Common.Interfaces;

public interface ICoursesRepository
{
    Task<Course?> GetCourseByIdAsync(int id);
    Task<IEnumerable<Course>> GetAllEnrolledCoursesAsync(string id);
    Task<IEnumerable<Course>> GetAllOwnedCoursesAsync(string id);
    Task<IEnumerable<Course>> GetAvailableToEnrollCoursesMatchingAsync(string? searchPhrase, string id);
}