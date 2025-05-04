using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omniwise.Application.UserCourses.Commands.AddPendingCourseMember;
using Omniwise.Domain.Constants;

namespace Omniwise.API.Controllers;

[ApiController]
[Route("api/courses/{courseId}/members")]
[Authorize(Roles = $"{Roles.Teacher},{Roles.Student}")]
public class UserCourseController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddPendingCourseMember([FromRoute] int courseId)
    {
        var command = new AddPendingCourseMemberCommand
        {
            CourseId = courseId
        };
        await mediator.Send(command);

        return NoContent();
    }
}
