using MediatR;

namespace Omniwise.Application.CourseMembers.Commands.AcceptCourseMember;

public class AcceptCourseMemberCommand : IRequest
{
    public int CourseId { get; set; }
    public string UserId { get; set; } = default!;
}
