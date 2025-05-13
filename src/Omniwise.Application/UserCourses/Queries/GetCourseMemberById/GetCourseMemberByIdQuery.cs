using MediatR;
using Omniwise.Application.UserCourses.Dtos;

namespace Omniwise.Application.UserCourses.Queries.GetCourseMemberById;

public class GetCourseMemberByIdQuery : IRequest<CourseMemberDto>
{
    public required int CourseId { get; init; }
    public required string MemberId { get; init; }
}
