using Omniwise.Infrastructure.Persistence;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Omniwise.Infrastructure.Repositories;

internal class LecturesRepository(OmniwiseDbContext dbContext) : ILecturesRepository
{
    public async Task<Lecture?> GetByIdAsync(int courseId, int lectureId)
    {
        var lecture = await dbContext.Lectures
            .FirstOrDefaultAsync(l => l.Id == lectureId
                                  && l.CourseId == courseId);
        return lecture;
    }
}
