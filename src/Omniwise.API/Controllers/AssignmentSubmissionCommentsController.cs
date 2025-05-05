using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omniwise.Application.AssignmentSubmissionComments.Commands.CreateAssignmentSubmissionComment;
using Omniwise.Domain.Constants;

namespace Omniwise.API.Controllers;

[ApiController]
[Route("api")]
[Authorize(Roles = $"{Roles.Teacher},{Roles.Student}")]
public class AssignmentSubmissionCommentsController(IMediator mediator) : ControllerBase
{
    [HttpPost("assignment-submissions/{assignmentSubmissionId}/assignment-submission-comments")]
    public async Task<IActionResult> CreateAssignmentSubmissionComment([FromBody] CreateAssignmentSubmissionCommentCommand command, [FromRoute] int assignmentSubmissionId)
    {
        command.AssignmentSubmissionId = assignmentSubmissionId;
        var assignmentSubmissionCommentId = await mediator.Send(command);

        var uri = $"assignment-submissions/{assignmentSubmissionId}/assignment-submission-comments/{assignmentSubmissionCommentId}";
        return Created(uri, null);
    }
}
