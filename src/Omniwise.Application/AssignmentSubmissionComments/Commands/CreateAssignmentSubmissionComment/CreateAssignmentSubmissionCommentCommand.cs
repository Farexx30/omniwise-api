using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.AssignmentSubmissionComments.Commands.CreateAssignmentSubmissionComment;

public class CreateAssignmentSubmissionCommentCommand : IRequest<int>
{
    public required string Content { get; init; }
    public int AssignmentSubmissionId { get; set; }
}
