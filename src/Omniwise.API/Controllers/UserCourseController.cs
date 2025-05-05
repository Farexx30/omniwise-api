using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omniwise.Application.UserCourses.Commands.AddPendingCourseMember;
using Omniwise.Application.UserCourses.Queries.GetEnrolledCourseMembers;
using Omniwise.Application.UserCourses.Queries.GetPendingCourseMembers;
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

    [HttpGet]
    [Route("pending")]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<IActionResult> GetPendingCourseMembers([FromRoute] int courseId)
    {
        var query = new GetPendingCourseMembersQuery { CourseId = courseId };
        var pendingCourseMembers = await mediator.Send(query);

        return Ok(pendingCourseMembers);
    }

    [HttpGet]
    [Route("enrolled")]
    public async Task<IActionResult> GetEnrolledMembers([FromRoute] int courseId)
    {
        var query = new GetEnrolledCourseMembersQuery { CourseId = courseId };
        var enrolledMembers = await mediator.Send(query);

        return Ok(enrolledMembers);
    }


}
