using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.AssignmentSubmissions.Commands.UpdateAssignmentSubmission;

public class UpdateAssignmentSubmissionCommand : IRequest
{
    public required List<IFormFile> Files { get; init; } = [];
    public int AssignmentSubmissionId { get; set; }
}
