using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omniwise.Application.AssignmentSubmissions.Commands.CreateAssignmentSubmission;
using Omniwise.Domain.Constants;

namespace Omniwise.API.Controllers;

[ApiController]
[Route("api")]
[Authorize(Roles = $"{Roles.Teacher},{Roles.Student}")]
public class AssignmentSubmissionsController(IMediator mediator) : ControllerBase
{
    [HttpPost("assignments/{assignmentId}/assignment-submissions")]
    [Authorize(Roles = Roles.Student)]
    public async Task<IActionResult> CreateAssignmentSubmission([FromForm] CreateAssignmentSubmissionCommand command, [FromRoute] int assignmentId)
    {
        command.AssignmentId = assignmentId;
        var assignmentSubmissionId = await mediator.Send(command);

        return CreatedAtAction(/*nameof(GetAssignmentSubmissionById)*/ null, new { assignmentSubmissionId }, null);
    }
}
