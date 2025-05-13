using MediatR;

namespace Omniwise.Application.CourseMembers.Commands.AddPendingCourseMember;

public class AddPendingCourseMemberCommand : IRequest
{
    public int CourseId { get; set; }
    public string UserId { get; set; } = default!;
    public bool IsAccepted { get; set; } = false;
}
