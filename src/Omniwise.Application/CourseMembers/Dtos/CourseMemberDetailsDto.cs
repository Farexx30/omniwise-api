using Omniwise.Application.Assignments.Dtos;
using Omniwise.Domain.Interfaces;

namespace Omniwise.Application.CourseMembers.Dtos;

public class CourseMemberDetailsDto : ICourseResource
{
    public string UserId { get; set; } = default!;
    public DateOnly? JoinDate { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string RoleName { get; set; } = default!;
    public List<AssignmentMemberSubmissionDto>? AssignmentSubmissions { get; set; }

    public int CourseId { get; set; }
}
