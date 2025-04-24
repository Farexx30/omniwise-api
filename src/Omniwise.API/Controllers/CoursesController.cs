using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omniwise.Application.Courses.Commands.CreateCourse;
using Omniwise.Application.Courses.Commands.UpdateCourse;
using Omniwise.Domain.Constants;

namespace Omniwise.API.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseCommand command)
    {
        int id = await mediator.Send(command);

        return CreatedAtAction(/*nameof(GetCourseById)*/ null, new { id }, null); //First argument waits for the GetCourseById action to exist.
    }

    [HttpPatch("{id}")]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<IActionResult> UpdateCourse([FromRoute] int id, [FromBody] UpdateCourseCommand command)
    {
        command.Id = id;
        await mediator.Send(command);

        return NoContent();
    }
}
