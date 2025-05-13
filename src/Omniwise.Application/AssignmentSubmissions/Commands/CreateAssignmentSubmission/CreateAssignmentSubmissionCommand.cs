using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.AssignmentSubmissions.Commands.CreateAssignmentSubmission;

public class CreateAssignmentSubmissionCommand : IRequest<int>
{
    public required List<IFormFile> Files { get; init; } = [];
    public int AssignmentId { get; set; }
}
