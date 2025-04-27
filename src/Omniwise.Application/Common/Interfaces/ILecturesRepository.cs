using Omniwise.Domain.Entities;

namespace Omniwise.Application.Common.Interfaces;

public interface ILecturesRepository
{
    Task<Lecture?> GetByIdAsync(int courseId, int lectureId);
}