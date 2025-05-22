using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    public async Task AddCourseMemberAsync(UserCourse courseMember)
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
        var currentUserId = currentUser.Id;
        var currentUserRoleName = currentUser.Roles.First();

        FormattableString query = $@"
            SELECT uc.UserId, uc.JoinDate, u.FirstName, u.LastName, u.Email, r.Name RoleName,
                   ad.AssignmentSubmissionId, ad.AssignmentName, ad.AssignmentDeadline, ad.AssignmentSubmissionGrade, 
                   uc.CourseId
            FROM UserCourses uc
            INNER JOIN AspNetUsers u ON uc.UserId = u.Id
            INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
            INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
            LEFT JOIN (
                SELECT asub.AuthorId, asub.Id AssignmentSubmissionId, a.Name AssignmentName, a.Deadline AssignmentDeadline, asub.Grade AssignmentSubmissionGrade
                FROM Assignments a
                INNER JOIN AssignmentSubmissions asub ON a.Id = asub.AssignmentId
                WHERE a.CourseId = {courseId}
            ) AS ad
            ON ad.AuthorId = {memberId}
            WHERE uc.UserId = {memberId} AND uc.CourseId = {courseId}";

        var flatResult = await dbContext.Database
            .SqlQuery<FlatCourseMemberDetailsDto>(query)
            .ToListAsync();

        var result = flatResult.GroupBy(flatResult => new
        {
            flatResult.UserId,
            flatResult.JoinDate,
            flatResult.FirstName,
            flatResult.LastName,
            flatResult.Email,
            flatResult.RoleName,
            flatResult.CourseId
        })
        .Select(groupResult => new CourseMemberDetailsDto
        {
            UserId = groupResult.Key.UserId,
            JoinDate = groupResult.Key.JoinDate,
            FirstName = groupResult.Key.FirstName,
            LastName = groupResult.Key.LastName,
            Email = groupResult.Key.Email,
            RoleName = groupResult.Key.RoleName,
            AssignmentSubmissions = (currentUserRoleName.Equals(Roles.Teacher, StringComparison.CurrentCultureIgnoreCase)
                                    || groupResult.Key.UserId.Equals(currentUserId))
                                    && !groupResult.Key.RoleName.Equals(Roles.Teacher, StringComparison.CurrentCultureIgnoreCase)
                ? [.. groupResult
                .Where(x => x.AssignmentSubmissionId != null)
                .Select(x => new AssignmentMemberSubmissionDto
                {
                    Id = x.AssignmentSubmissionId!.Value,
                    Name = x.AssignmentName!,
                    Deadline = x.AssignmentDeadline!.Value,
                    Grade = x.AssignmentSubmissionGrade
                })]
                : default,
            CourseId = groupResult.Key.CourseId
        })
        .FirstOrDefault();

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
    public async Task<IEnumerable<EnrolledCourseMemberWithRoleDto>> GetEnrolledCourseMembersWithRolesAsync(int courseId)
    {
        var enrolledCourseMembers = await dbContext.UserCourses
            .Include(uc => uc.User)
            .Where(uc => uc.CourseId == courseId && uc.IsAccepted == true)
            .Join(dbContext.UserRoles,
                  member => member.UserId,
                  userRole => userRole.UserId,
                  (member, userRole) => new { Member = member, UserRole = userRole })
            .Join(dbContext.Roles,
                  firstJoinResult => firstJoinResult.UserRole.RoleId,
                  role => role.Id,
                  (firstJoinResult, role) => new EnrolledCourseMemberWithRoleDto
                  {
                      UserId = firstJoinResult.Member.UserId,
                      FirstName = firstJoinResult.Member.User.FirstName,
                      LastName = firstJoinResult.Member.User.LastName,
                      RoleName = role.Name!
                  })
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

    public async Task<EnrolledCourseMemberWithRoleDto?> GetCourseMemberWithRoleNameAsync(int courseId, string userId)
    {
        var courseMember = await dbContext.UserCourses
            .Join(dbContext.UserRoles,
                  userCourse => userCourse.UserId,
                  userRole => userRole.UserId,
                  (userCourse, userRole) => new { UserCourse = userCourse, UserRole = userRole })
            .Join(dbContext.Roles,
                  firstJoinResult => firstJoinResult.UserRole.RoleId,
                  role => role.Id,
                  (firstJoinResult, role) => new { firstJoinResult.UserCourse, Role = role })
            .Where(secondJoinResult => secondJoinResult.UserCourse.CourseId == courseId 
                   && secondJoinResult.UserCourse.UserId == userId)
            .Select(secondJoinResult => new EnrolledCourseMemberWithRoleDto
            {
                UserId = secondJoinResult.UserCourse.UserId,
                RoleName = secondJoinResult.Role.Name!
            })
            .FirstOrDefaultAsync();

        return courseMember;
    }

    public async Task DeleteByUserIdAsync(string userId)
    {
        await dbContext.UserCourses
            .Where(uc => uc.UserId == userId)
            .ExecuteDeleteAsync();
    }

    public async Task<List<string>> GetTeacherIdsAsync(int courseId)
    {
        var result = await dbContext.UserCourses
            .Include(uc => uc.User)
            .Where(uc => uc.CourseId == courseId && uc.IsAccepted == true)
            .Join(dbContext.UserRoles,
                  member => member.UserId,
                  userRole => userRole.UserId,
                  (member, userRole) => new { Member = member, UserRole = userRole })
            .Join(dbContext.Roles,
                  firstJoinResult => firstJoinResult.UserRole.RoleId,
                  role => role.Id,
                  (firstJoinResult, role) => new { firstJoinResult.Member.UserId, Role = role })
            .Where(result => result.Role.Name == Roles.Teacher)
            .Select(result => result.UserId)
            .ToListAsync();

        return result;
    }
}
