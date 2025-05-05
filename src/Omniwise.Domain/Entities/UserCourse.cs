using Omniwise.Domain.Interfaces;

namespace Omniwise.Domain.Entities;

public class UserCourse : ICourseResource
{
    public string UserId { get; set; } = default!;
    public int CourseId { get; set; }
    public DateOnly? JoinDate { get; set; }
    public bool IsAccepted { get; set; }

    //References:
    public User User { get; set; } = default!;
}
