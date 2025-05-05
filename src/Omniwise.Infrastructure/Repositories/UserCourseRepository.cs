using Microsoft.EntityFrameworkCore;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Persistence;

namespace Omniwise.Infrastructure.Repositories;

internal class UserCourseRepository(OmniwiseDbContext dbContext) : IUserCourseRepository
{
    public async Task AddPendingCourseMemberAsync(UserCourse courseMember)
    {
        dbContext.UserCourses.Add(courseMember);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int courseId, string userId)
    {
        return await dbContext.UserCourses
            .AnyAsync(uc => uc.CourseId == courseId && uc.UserId == userId);
    }

    public async Task<IEnumerable<UserCourse>> GetPendingCourseMembersAsync(int courseId)
    {
        var pendingCourseMembers = await dbContext.UserCourses
            .Include(uc => uc.User)
            .Where(uc => uc.CourseId == courseId && uc.IsAccepted == false)
            .ToListAsync();

        return pendingCourseMembers;
    }
}
