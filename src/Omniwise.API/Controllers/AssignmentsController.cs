using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omniwise.Application.Assignments.Dtos;
using Omniwise.Application.Assignments.Queries.GetAssignmentById;
using Omniwise.Domain.Constants;

namespace Omniwise.API.Controllers;

[ApiController]
[Route("api/courses/{courseId}/assignments")]
[Authorize(Roles = $"{Roles.Teacher},{Roles.Student}")]
public class AssignmentsController(IMediator mediator) : ControllerBase
{
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
}
