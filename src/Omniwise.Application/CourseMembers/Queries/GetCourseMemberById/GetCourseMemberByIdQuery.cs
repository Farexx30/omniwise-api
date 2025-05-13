using MediatR;
using Omniwise.Application.CourseMembers.Dtos;

namespace Omniwise.Application.CourseMembers.Queries.GetCourseMemberById;

public class GetCourseMemberByIdQuery : IRequest<CourseMemberDetailsDto>
{
    public required int CourseId { get; init; }
    public required string MemberId { get; init; }
}
