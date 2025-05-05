using MediatR;
using Omniwise.Application.UserCourses.Dtos;

namespace Omniwise.Application.UserCourses.Queries.GetEnrolledCourseMembers;

public class GetEnrolledCourseMembersQuery : IRequest<IEnumerable<EnrolledUserCourseDto>>
{
    public required int CourseId { get; init; }
}