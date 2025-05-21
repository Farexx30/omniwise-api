using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omniwise.Application.Courses.Commands.CreateCourse;
using Omniwise.Application.Courses.Commands.DeleteCourse;
using Omniwise.Application.Courses.Commands.UpdateCourse;
using Omniwise.Application.Courses.Queries.GetAvailableToEnrollCourses;
using Omniwise.Application.Courses.Queries.GetCourseById;
using Omniwise.Application.Courses.Queries.GetEnrolledCourses;
using Omniwise.Application.Courses.Queries.GetOwnedCourses;
using Omniwise.Domain.Constants;

namespace Omniwise.API.Controllers;

[ApiController]
[Route("api/courses")]
[Authorize(Roles = $"{Roles.Teacher},{Roles.Student}")]
public class CoursesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<IActionResult> CreateCourse([FromForm] CreateCourseCommand command)
    {
        int courseId = await mediator.Send(command);

        return CreatedAtAction(nameof(GetCourseById), new { courseId }, null);
    }

    [HttpPatch("{courseId}")]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<IActionResult> UpdateCourse([FromForm] UpdateCourseCommand command, [FromRoute] int courseId)
    {
        command.Id = courseId;
        await mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{courseId}")]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<IActionResult> DeleteCourse([FromRoute] int courseId)
    {
        var command = new DeleteCourseCommand { Id = courseId };
        await mediator.Send(command);

        return NoContent();
    }


    [HttpGet("{courseId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseById([FromRoute] int courseId)
    {
        var course = await mediator.Send(new GetCourseByIdQuery(courseId));
        return Ok(course);
    }

    [HttpGet("enrolled")]
    public async Task<IActionResult> GetEnrolledCourses()
    {
        var enrolledCourses = await mediator.Send(new GetEnrolledCoursesQuery());
        return Ok(enrolledCourses);
    }

    [HttpGet("owned")]
    public async Task<IActionResult> GetOwnedCourses()
    {
        var ownedCourses = await mediator.Send(new GetOwnedCoursesQuery());
        return Ok(ownedCourses);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableToEnrollCourses([FromQuery] GetAvailableToEnrollCoursesQuery query)
    {
        var availableCourses = await mediator.Send(query);
        return Ok(availableCourses);
    }
}