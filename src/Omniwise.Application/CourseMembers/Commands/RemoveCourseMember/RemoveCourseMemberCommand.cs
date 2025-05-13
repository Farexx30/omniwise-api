using MediatR;

namespace Omniwise.Application.CourseMembers.Commands.RemoveCourseMember;

public class RemoveCourseMemberCommand : IRequest
{
    public required string UserId { get; init; }
    public required int CourseId { get; init; }
}
