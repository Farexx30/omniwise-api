using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.AssignmentSubmissionComments.Commands.UpdateAssignmentSubmissionComment;

public class UpdateAssignmentSubmissionCommentCommand : IRequest
{
    public required string Content { get; init; }
    public int AssignmentSubmissionCommentId { get; set; }
}
