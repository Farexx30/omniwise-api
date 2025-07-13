using Omniwise.Application.Users.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.AssignmentSubmissionComments.Dtos;

public class AssignmentSubmissionCommentDto
{
    public int Id { get; set; }
    public string Content { get; set; } = default!;
    public DateTime SentDate { get; set; }
    public string AuthorId { get; set; } = default!;
    public string AuthorFullName { get; set; } = default!;
}
