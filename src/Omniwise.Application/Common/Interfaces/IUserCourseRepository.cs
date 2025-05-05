using Omniwise.Domain.Entities;

namespace Omniwise.Application.Common.Interfaces;

public interface IUserCourseRepository
{
    Task<bool> ExistsAsync(int courseId, string userId);
    Task AddPendingCourseMemberAsync(UserCourse courseMember);
    Task<IEnumerable<UserCourse>> GetPendingCourseMembersAsync(int courseId);
    Task<IEnumerable<UserCourse>> GetEnrolledCourseMembersAsync(int courseId);
}