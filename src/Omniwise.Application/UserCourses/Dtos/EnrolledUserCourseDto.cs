namespace Omniwise.Application.UserCourses.Dtos;

public class EnrolledUserCourseDto
{
    public string UserId { get; set; } = default!;
    public int CourseId { get; set; }
    public bool IsAccepted { get; set; }
    public DateOnly? JoinDate { get; set; }

    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
}
