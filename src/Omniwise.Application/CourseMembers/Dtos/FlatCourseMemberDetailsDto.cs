using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.CourseMembers.Dtos;

public class FlatCourseMemberDetailsDto
{
    public string UserId { get; set; } = default!;
    public DateOnly? JoinDate { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string RoleName { get; set; } = default!;
    public int CourseId { get; set; }
    public int? AssignmentSubmissionId { get; set; }
    public string? AssignmentName { get; set; }
    public DateTime? AssignmentDeadline { get; set; }
    public float? AssignmentSubmissionGrade { get; set; }
}
