using Omniwise.Application.AssignmentSubmissions.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Assignments.Dtos;

public class AssignmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Content { get; set; }
    public DateTime Deadline { get; set; }
    public float MaxGrade { get; set; }
    public List<string> FileUrls { get; set; } = [];
    public IEnumerable<BasicAssignmentSubmissionDto> Submissions { get; set; } = [];
}
