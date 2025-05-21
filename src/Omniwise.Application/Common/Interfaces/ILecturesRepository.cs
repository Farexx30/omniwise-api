using Omniwise.Domain.Entities;

namespace Omniwise.Application.Common.Interfaces;

public interface ILecturesRepository
{
    Task<int> CreateAsync(Lecture lecture);
    Task DeleteAsync(Lecture lecture);
    Task<IEnumerable<Lecture>> GetAllLecturesAsync(int courseId);
    Task<Lecture?> GetByIdAsync(int courseId, int lectureId);
    Task<IEnumerable<int>> GetAllIdsByCourseIdAsync(int courseId);
    Task SaveChangesAsync();
}