using MediatR;
using Omniwise.Application.CourseMembers.Dtos;

namespace Omniwise.Application.CourseMembers.Queries.GetPendingCourseMembers;

public class GetPendingCourseMembersQuery : IRequest<IEnumerable<PendingCourseMemberDto>>
{
    public required int CourseId { get; init; }
}
