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

namespace Omniwise.API.Controllers;

[ApiController]
[Route("api/courses/{courseId}/assignments")]
[Authorize(Roles = $"{Roles.Teacher},{Roles.Student}")]
public class AssignmentsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentCommand command, [FromRoute] int courseId)
    {
        command.CourseId = courseId;
        var assignmentId = await mediator.Send(command);

        return CreatedAtAction(nameof(GetAssignmentById), new { assignmentId, courseId }, null);
    }

    [HttpPatch("{assignmentId}")]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<IActionResult> UpdateAssignment([FromBody] UpdateAssignmentCommand command, [FromRoute] int assignmentId, [FromRoute] int courseId)
    {
        command.AssignmentId = assignmentId;
        command.CourseId = courseId;
        await mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{assignmentId}")]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<IActionResult> DeleteAssignment([FromRoute] int assignmentId, [FromRoute] int courseId)
    {
        var command = new DeleteAssignmentCommand 
        { 
            AssignmentId = assignmentId,
            CourseId = courseId 
        };
        await mediator.Send(command);

        return NoContent();
    }

    [HttpGet("{assignmentId}")]
    public async Task<ActionResult<AssignmentDto>> GetAssignmentById([FromRoute] int assignmentId, [FromRoute] int courseId)
    {
        var query = new GetAssignmentByIdQuery 
        { 
            AssignmentId = assignmentId,
            CourseId = courseId
        };
        var assignment = await mediator.Send(query);

        return Ok(assignment);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BasicAssignmentDto>>> GetAllCourseAssignments([FromRoute] int courseId)
    {
        var query = new GetAllCourseAssignmentsQuery { CourseId = courseId };
        var assignments = await mediator.Send(query);

        return Ok(assignments);
    }
}
