using MediatR;
using Omniwise.Application.CourseMembers.Dtos;

namespace Omniwise.Application.CourseMembers.Queries.GetEnrolledCourseMembers;

public class GetEnrolledCourseMembersQuery : IRequest<IEnumerable<EnrolledCourseMemberDto>>
{
    public required int CourseId { get; init; }
}