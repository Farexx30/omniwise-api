using Microsoft.EntityFrameworkCore;
using Omniwise.Application.Common.Types;
using Omniwise.Application.CourseMembers.Dtos;
using Omniwise.Domain.Entities;

namespace Omniwise.Application.Common.Interfaces.Repositories;

public interface IUserCourseRepository
{
    Task<bool> ExistsAsync(int courseId, string userId);
    Task AddCourseMemberAsync(UserCourse courseMember);
    Task<IEnumerable<UserCourse>> GetPendingCourseMembersAsync(int courseId);
    Task<IEnumerable<UserCourse>> GetEnrolledCourseMembersAsync(int courseId);
    Task<IEnumerable<EnrolledCourseMemberWithRoleDto>> GetEnrolledCourseMembersWithRolesAsync(int courseId);
    Task<CourseMemberDetailsDto?> GetByIdAsync(string memberId, int courseId, CurrentUser currentUser);
    Task<UserCourse?> GetPendingCourseMemberAsync(int courseId, string userId);
    Task<EnrolledCourseMemberWithRoleDto?> GetCourseMemberWithRoleNameAsync(int courseId, string userId);
    Task DeleteByUserIdAsync(string userId);
    Task<List<string>> GetTeacherIdsAsync(int courseId);
    Task SaveChangesAsync();
}