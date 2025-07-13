using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omniwise.Application.Assignments.Commands.CreateAssignment;
using Omniwise.Application.Assignments.Commands.DeleteAssignment;
using Omniwise.Application.Assignments.Commands.UpdateAssignment;
using Omniwise.Application.Assignments.Dtos;
using Omniwise.Application.Assignments.Queries.GetAllCourseAssignments;
using Omniwise.Application.Assignments.Queries.GetAssignmentById;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;

namespace Omniwise.API.Controllers;

[ApiController]
[Route("api")]
[Authorize(Roles = $"{Roles.Teacher},{Roles.Student}")]
public class AssignmentsController(IMediator mediator) : ControllerBase
{
    [HttpPost("courses/{courseId}/assignments")]
    [Authorize(Roles = Roles.Teacher)]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> CreateAssignment([FromForm] CreateAssignmentCommand command, [FromRoute] int courseId)
    {
        command.CourseId = courseId;
        var assignmentId = await mediator.Send(command);

        return CreatedAtAction(nameof(GetAssignmentById), new { assignmentId }, new { assignmentId });
    }

    [HttpPatch("assignments/{assignmentId}")]
    [Authorize(Roles = Roles.Teacher)]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> UpdateAssignment([FromForm] UpdateAssignmentCommand command, [FromRoute] int assignmentId)
    {
        command.AssignmentId = assignmentId;
        await mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("assignments/{assignmentId}")]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<IActionResult> DeleteAssignment([FromRoute] int assignmentId)
    {
        var command = new DeleteAssignmentCommand { AssignmentId = assignmentId };
        await mediator.Send(command);

        return NoContent();
    }

    [HttpGet("assignments/{assignmentId}")]
    public async Task<ActionResult<AssignmentDto>> GetAssignmentById([FromRoute] int assignmentId)
    {
        var query = new GetAssignmentByIdQuery { AssignmentId = assignmentId };
        var assignment = await mediator.Send(query);

        return Ok(assignment);
    }

    [HttpGet("courses/{courseId}/assignments")]
    public async Task<ActionResult<IEnumerable<BasicAssignmentDto>>> GetAllCourseAssignments([FromRoute] int courseId)
    {
        var query = new GetAllCourseAssignmentsQuery { CourseId = courseId };
        var assignments = await mediator.Send(query);

        return Ok(assignments);
    }
}
