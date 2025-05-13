using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Domain.Entities;

public class AssignmentSubmission
{
    public int Id { get; set; }
    public float? Grade { get; set; }
    public DateTime LatestSubmissionDate { get; set; }
    public int AssignmentId { get; set; }
    public string? AuthorId { get; set; }

    //References:
    public Assignment Assignment { get; set; } = default!;
    public List<AssignmentSubmissionComment> Comments { get; set; } = [];
    public List<AssignmentSubmissionFile> Files { get; set; } = [];
}
