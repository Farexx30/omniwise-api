namespace Omniwise.Application.CourseMembers.Dtos;

public class CourseMemberDto
{
    public string UserId { get; set; } = default!;
    public int CourseId { get; set; }
    public DateOnly? JoinDate { get; set; }
    public bool IsAccepted { get; set; }
}
