namespace Omniwise.Application.CourseMembers.Dtos;

public class EnrolledCourseMemberWithRoleDto
{
    public string UserId { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string RoleName { get; set; } = default!;

}
