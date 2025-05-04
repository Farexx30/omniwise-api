namespace Omniwise.Application.UserCourses.Dtos;

public class UserCourseDto
{
    public string UserId { get; set; } = default!;
    public int CourseId { get; set; }
    public DateOnly? JoinDate { get; set; }
    public bool IsAccepted { get; set; }
}
