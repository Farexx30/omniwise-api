namespace Omniwise.Application.CourseMembers.Dtos;

public class PendingCourseMemberDto
{
    public string UserId { get; set; } = default!;
    public int CourseId { get; set; }
    public bool IsAccepted { get; set; }

    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
}
