using Omniwise.Domain.Entities;

namespace Omniwise.Application.Common.Interfaces.Repositories;

public interface ILecturesRepository
{
    Task<int> CreateAsync(Lecture lecture);
    Task DeleteAsync(Lecture lecture);
    Task<IEnumerable<Lecture>> GetAllCourseLecturesAsync(int courseId);
    Task<Lecture?> GetByIdAsync(int lectureId);
    Task<IEnumerable<int>> GetAllIdsByCourseIdAsync(int courseId);
    Task SaveChangesAsync();
}