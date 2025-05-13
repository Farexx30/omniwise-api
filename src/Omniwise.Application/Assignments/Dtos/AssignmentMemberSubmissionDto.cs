namespace Omniwise.Application.Assignments.Dtos;

public class AssignmentMemberSubmissionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime Deadline { get; set; }
    public float? Grade { get; set; }
}
