using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.AssignmentSubmissionComments.Commands.DeleteAssignmentSubmissionComment;

public class DeleteAssignmentSubmissionCommentCommand : IRequest
{
    public required int AssignmentSubmissionCommentId { get; init; }
}
