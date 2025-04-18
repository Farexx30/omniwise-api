using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Domain.Entities;

public class AssignmentSubmissionComment
{
    public int Id { get; set; }
    public string Content { get; set; } = default!;
    public DateTime SentDate { get; set; }

    public int AssignmentSubmissionId { get; set; }
    public AssignmentSubmission AssignmentSubmission { get; set; } = default!;

    public string AuthorId { get; set; } = default!;
    public User Author { get; set; } = default!;
}
