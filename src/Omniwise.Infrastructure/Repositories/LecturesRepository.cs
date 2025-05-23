using Omniwise.Infrastructure.Persistence;
using Omniwise.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Omniwise.Application.Common.Interfaces.Repositories;

namespace Omniwise.Infrastructure.Repositories;

internal class LecturesRepository(OmniwiseDbContext dbContext) : ILecturesRepository
{
    public async Task<int> CreateAsync(Lecture lecture)
    {
        dbContext.Lectures.Add(lecture);
        await dbContext.SaveChangesAsync();

        return lecture.Id;
    }

    public async Task DeleteAsync(Lecture lecture)
    {
        dbContext.Lectures.Remove(lecture);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Lecture?> GetByIdAsync(int lectureId)
    {
        var lecture = await dbContext.Lectures
            .Include(l => l.Files)
            .FirstOrDefaultAsync(l => l.Id == lectureId);

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

    public async Task<IEnumerable<Lecture>> GetAllCourseLecturesAsync(int courseId)
    {
        var lectures = await dbContext.Lectures
            .AsNoTracking()
            .Where(l => l.CourseId == courseId)
            .ToListAsync();

        return lectures;
    }

    public Task SaveChangesAsync()
        => dbContext.SaveChangesAsync();
}
