using Omniwise.Domain.Entities;

namespace Omniwise.Application.Common.Interfaces;

public interface IUserCourseRepository
{
    Task<bool> ExistsAsync(int courseId, string userId);
    Task AddPendingCourseMemberAsync(UserCourse courseMember);
}