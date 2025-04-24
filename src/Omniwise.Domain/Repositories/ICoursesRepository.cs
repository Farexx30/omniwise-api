using Omniwise.Domain.Entities;

namespace Omniwise.Domain.Repositories;

public interface ICoursesRepository
{
    Task<Course?> GetCourseByIdAsync(int id);
}