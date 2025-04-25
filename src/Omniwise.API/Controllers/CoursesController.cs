using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omniwise.Application.Courses.Queries;
using Omniwise.Domain.Constants;

namespace Omniwise.API.Controllers;

[ApiController]
[Route("api/courses")]
[Authorize(Roles = $"{Roles.Teacher},{Roles.Student}")]
public class CoursesController(IMediator mediator) : ControllerBase
{
    [HttpGet("{courseId}")]
    public async Task<IActionResult> GetCourseById([FromRoute] int courseId)
    {
        var course = await mediator.Send(new GetCourseByIdQuery(courseId));
        return Ok(course);
    }
}
