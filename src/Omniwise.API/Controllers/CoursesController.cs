using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [HttpGet("{courseId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseById([FromRoute] int courseId)
    {
        var course = await mediator.Send(new GetCourseByIdQuery(courseId));
        return Ok(course);
    }

    [HttpGet("enrolled/{userId}")]
    public async Task<IActionResult> GetEnrolledCourses([FromRoute] string userId)
    {
        var enrolledCourses = await mediator.Send(new GetEnrolledCoursesQuery(userId));
        return Ok(enrolledCourses);
    }

    [HttpGet("owned/{userId}")]
    public async Task<IActionResult> GetOwnedCourses([FromRoute] string userId)
    {
        var ownedCourses = await mediator.Send(new GetOwnedCoursesQuery(userId));
        return Ok(ownedCourses);
    }
    [AllowAnonymous]
    [HttpGet("available/{userId}")]
    public async Task<IActionResult> GetAvailableToEnrollCourses([FromRoute] string userId, [FromQuery] GetAvailableToEnrollCoursesQuery query)
    {
        query.Id = userId;
        var availableCourses = await mediator.Send(query);
        return Ok(availableCourses);
    }
}
