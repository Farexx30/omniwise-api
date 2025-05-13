using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.AssignmentSubmissions.Commands.DeleteAssignmentSubmission;

public class DeleteAssignmentSubmissionCommand : IRequest
{
    public required int AssignmentSubmissionId { get; init; }
}
