using MediatR;
using Omniwise.Application.UserCourses.Dtos;

namespace Omniwise.Application.UserCourses.Queries.GetPendingCourseMembers;

public class GetPendingCourseMembersQuery : IRequest<IEnumerable<PendingUserCourseDto>>
{
    public required int CourseId { get; init; }
}
