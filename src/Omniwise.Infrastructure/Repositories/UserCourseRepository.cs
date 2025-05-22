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
        var isTheSameUser = currentUser.Id!.Equals(memberId, StringComparison.CurrentCultureIgnoreCase);
        var isCurrentUserTeacher = currentUser.IsInRole(Roles.Teacher);

        var mainQuery = dbContext.UserCourses
            .Where(uc => uc.UserId == memberId
                   && uc.CourseId == courseId)
            .Join(dbContext.UserRoles,
                  userCourse => userCourse.UserId,
                  userRole => userRole.UserId,
                  (userCourse, userRole) => new { UserCourse = userCourse, UserRole = userRole })
            .Join(dbContext.Roles,
                  currentResult => currentResult.UserRole.RoleId,
                  role => role.Id,
                  (currentResult, role) => new { currentResult.UserCourse, Role = role, AssignmentsWithSubmissions = Enumerable.Empty<AssignmentMemberSubmissionDto>() });

        bool shouldIncludeAssignmentSubmissions = isCurrentUserTeacher || isTheSameUser;
        if (shouldIncludeAssignmentSubmissions)
        {
            var assignmentsWithSubmissionsSubquery = dbContext.AssignmentSubmissions
                    .Where(asub => asub.Assignment.CourseId == courseId)
                    .Select(asub => new
                    {
                        asub.AuthorId,
                        asub.Id,
                        asub.Assignment.Name,
                        asub.LatestSubmissionDate,
                        asub.Assignment.Deadline,
                        asub.Grade,
                    });

            mainQuery = mainQuery
                .GroupJoin(assignmentsWithSubmissionsSubquery,
                       currentResult => currentResult.UserCourse.UserId,
                       subquery => subquery.AuthorId,
                       (currentResult, subquery) => new 
                       {
                           currentResult.UserCourse,
                           currentResult.Role,
                           AssignmentsWithSubmissions = subquery.Select(x => new AssignmentMemberSubmissionDto
                           {
                               Id = x.Id,
                               Name = x.Name,
                               LatestSubmissionDate = x.LatestSubmissionDate,
                               Deadline = x.Deadline,
                               Grade = x.Grade
                           }) 
                       });
        }

        var result = await mainQuery.Select(finalResult => new CourseMemberDetailsDto
        {
            UserId = finalResult.UserCourse.UserId,
            JoinDate = finalResult.UserCourse.JoinDate,
            FirstName = finalResult.UserCourse.User.FirstName,
            LastName = finalResult.UserCourse.User.LastName,
            Email = finalResult.UserCourse.User.Email!,
            RoleName = finalResult.Role.Name!,
            AssignmentSubmissions = shouldIncludeAssignmentSubmissions && finalResult.Role.Name != Roles.Teacher
                ? finalResult.AssignmentsWithSubmissions
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
