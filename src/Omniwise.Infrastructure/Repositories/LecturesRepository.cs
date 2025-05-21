using Omniwise.Infrastructure.Persistence;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Omniwise.Infrastructure.Repositories;

internal class LecturesRepository(OmniwiseDbContext dbContext) : ILecturesRepository
{
    public async Task<int> CreateAsync(Lecture lecture)
    {
        dbContext.Lectures.Add(lecture);
        await dbContext.SaveChangesAsync();

        return lecture.Id;
    }

    public Task SaveChangesAsync() => dbContext.SaveChangesAsync();

    public async Task DeleteAsync(Lecture lecture)
    {
        dbContext.Lectures.Remove(lecture);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Lecture?> GetByIdAsync(int courseId, int lectureId)
    {
        var lecture = await dbContext.Lectures
            .Include(l => l.Files)
            .FirstOrDefaultAsync(l => l.Id == lectureId
                                  && l.CourseId == courseId);

        return lecture;
    }

    public async Task<IEnumerable<int>> GetAllIdsByCourseIdAsync(int courseId)
    {
        var lectureIds = await dbContext.Lectures
            .Where(l => l.CourseId == courseId)
            .Select(l => l.Id)
            .ToListAsync();

        return lectureIds;
    }

    public async Task<IEnumerable<Lecture>> GetAllLecturesAsync(int courseId)
    {
        var lectures = await dbContext.Lectures
            .Where(l => l.CourseId == courseId)
            .ToListAsync();

        return lectures;
    }
}
