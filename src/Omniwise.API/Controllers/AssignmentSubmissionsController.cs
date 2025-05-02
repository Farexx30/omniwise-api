using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omniwise.Application.AssignmentSubmissions.Commands.CreateAssignmentSubmission;
using Omniwise.Application.AssignmentSubmissions.Commands.DeleteAssignmentSubmission;
using Omniwise.Application.AssignmentSubmissions.Dtos;
using Omniwise.Application.AssignmentSubmissions.Queries.GetAssignmentSubmissionById;
using Omniwise.Domain.Constants;

namespace Omniwise.API.Controllers;

[ApiController]
[Route("api")]
[Authorize(Roles = $"{Roles.Teacher},{Roles.Student}")]
public class AssignmentSubmissionsController(IMediator mediator) : ControllerBase
{
    [HttpPost("assignments/{assignmentId}/assignment-submissions")]
    [Authorize(Roles = Roles.Student)]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> CreateAssignmentSubmission([FromForm] CreateAssignmentSubmissionCommand command, [FromRoute] int assignmentId)
    {
        command.AssignmentId = assignmentId;
        var assignmentSubmissionId = await mediator.Send(command);

        return CreatedAtAction(nameof(GetAssignmentSubmissionById), new { assignmentSubmissionId }, null);
    }

    [HttpDelete("assignment-submissions/{assignmentSubmissionId}")]
    [Authorize(Roles = Roles.Student)]
    public async Task<IActionResult> DeleteAssignmentSubmission([FromRoute] int assignmentSubmissionId)
    {
        var command = new DeleteAssignmentSubmissionCommand { AssignmentSubmissionId = assignmentSubmissionId };
        await mediator.Send(command);

        return NoContent();
    }

    [HttpGet("assignment-submissions/{assignmentSubmissionId}")]
    public async Task<ActionResult<AssignmentSubmissionDto>> GetAssignmentSubmissionById([FromRoute] int assignmentSubmissionId)
    {
        var query = new GetAssignmentSubmissionByIdQuery { AssignmentSubmissionId = assignmentSubmissionId };
        var assignmentSubmission = await mediator.Send(query);

        return Ok(assignmentSubmission);
    }
}
