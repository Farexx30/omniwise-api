using Microsoft.EntityFrameworkCore;
using Omniwise.Application.Assignments.Dtos;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Common.Types;
using Omniwise.Application.CourseMembers.Dtos;
using Omniwise.Domain.Constants;
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

    public async Task<CourseMemberDetailsDto?> GetByIdAsync(string memberId, int courseId, CurrentUser currentUser)
    {
        var currentUserId = currentUser.Id!;
        var currentUserRoleName = currentUser.Roles.First();

        var result = await dbContext.Users
             .Where(u => u.Id == memberId)
             .Include(member => member.UserCourses)
             .Include(member => member.AssignmentSubmissions)
                    .ThenInclude(a => a.Assignment)
            .Join(dbContext.UserRoles,
                  member => member.Id,
                  userRole => userRole.UserId,
                  (member, userRole) => new { Member = member, UserRole = userRole })
            .Join(dbContext.Roles,
                  firstJoinResult => firstJoinResult.UserRole.RoleId,
                  role => role.Id,
                  (firstJoinResult, role) => new { firstJoinResult.Member, Role = role })
            .Select(result => new CourseMemberDetailsDto
            {
                UserId = result.Member.Id,
                JoinDate = result.Member.UserCourses
                    .Where(uc => uc.CourseId == courseId)
                    .Select(uc => uc.JoinDate)
                    .First(),
                FirstName = result.Member.FirstName,
                LastName = result.Member.LastName,
                Email = result.Member.Email!,
                RoleName = result.Role.Name!,
                AssignmentSubmissions = currentUserRoleName.Equals(Roles.Teacher, StringComparison.CurrentCultureIgnoreCase)
                                        || currentUserId.Equals(memberId, StringComparison.CurrentCultureIgnoreCase)
                ? result.Member.AssignmentSubmissions.Select(asub => new AssignmentMemberSubmissionDto
                {
                    Id = asub.Id,
                    Name = asub.Assignment.Name,
                    Deadline = asub.Assignment.Deadline,
                    Grade = asub.Grade,
                })
                .ToList()
                : default,
                CourseId = courseId
            })
            .FirstOrDefaultAsync();

        return result;
    }

    public async Task<IEnumerable<UserCourse>> GetEnrolledCourseMembersAsync(int courseId)
    {
        var enrolledCourseMembers = await dbContext.UserCourses
            .Include(uc => uc.User)
            .Where(uc => uc.CourseId == courseId && uc.IsAccepted == true)
            .ToListAsync();

        return enrolledCourseMembers;
    }

    public async Task<UserCourse?> GetPendingCourseMemberAsync(int courseId, string userId)
    {
        var pendingCourseMember = await dbContext.UserCourses
            .FirstOrDefaultAsync(uc => uc.CourseId == courseId && uc.UserId == userId && uc.IsAccepted == false);

        return pendingCourseMember;
    }

    public async Task<IEnumerable<UserCourse>> GetPendingCourseMembersAsync(int courseId)
    {
        var pendingCourseMembers = await dbContext.UserCourses
            .Include(uc => uc.User)
            .Where(uc => uc.CourseId == courseId && uc.IsAccepted == false)
            .ToListAsync();

        return pendingCourseMembers;
    }

    public Task SaveChangesAsync()
        => dbContext.SaveChangesAsync();

    public async Task<UserCourse?> GetCourseMemberAsync(int courseId, string userId)
    {
        var courseMember = await dbContext.UserCourses
            .FirstOrDefaultAsync(uc => uc.CourseId == courseId && uc.UserId == userId);

        return courseMember;
    }

    public async Task DeleteAsync(UserCourse courseMember)
    {
        dbContext.UserCourses.Remove(courseMember);
        await dbContext.SaveChangesAsync();
    }

}
