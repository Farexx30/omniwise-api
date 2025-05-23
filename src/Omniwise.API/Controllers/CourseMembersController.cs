using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omniwise.Application.CourseMembers.Commands.AcceptCourseMember;
using Omniwise.Application.CourseMembers.Commands.AddPendingCourseMember;
using Omniwise.Application.CourseMembers.Commands.RemoveCourseMember;
using Omniwise.Application.CourseMembers.Dtos;
using Omniwise.Application.CourseMembers.Queries.GetCourseMemberById;
using Omniwise.Application.CourseMembers.Queries.GetEnrolledCourseMembers;
using Omniwise.Application.CourseMembers.Queries.GetPendingCourseMembers;
using Omniwise.Domain.Constants;

namespace Omniwise.API.Controllers;

[ApiController]
[Route("api/courses/{courseId}/members")]
[Authorize(Roles = $"{Roles.Teacher},{Roles.Student}")]
public class CourseMembersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddPendingCourseMember([FromRoute] int courseId)
    {
        var command = new AddPendingCourseMemberCommand { CourseId = courseId };
        await mediator.Send(command);

        return NoContent();
    }

    [HttpGet]
    [Route("pending")]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<ActionResult<IEnumerable<PendingCourseMemberDto>>> GetPendingCourseMembers([FromRoute] int courseId)
    {
        var query = new GetPendingCourseMembersQuery { CourseId = courseId };
        var pendingCourseMembers = await mediator.Send(query);

        return Ok(pendingCourseMembers);
    }

    [HttpGet]
    [Route("enrolled")]
    public async Task<ActionResult<IEnumerable<EnrolledCourseMemberDto>>> GetEnrolledMembers([FromRoute] int courseId)
    {
        var query = new GetEnrolledCourseMembersQuery { CourseId = courseId };
        var enrolledMembers = await mediator.Send(query);

        return Ok(enrolledMembers);
    }

    [HttpGet]
    [Route("{memberId}")]
    public async Task<ActionResult<CourseMemberDetailsDto>> GetCourseMemberById([FromRoute] string memberId, [FromRoute] int courseId)
    {
        var query = new GetCourseMemberByIdQuery
        {
            MemberId = memberId,
            CourseId = courseId
        };
        var courseMember = await mediator.Send(query);

        return Ok(courseMember);
    }

    [HttpPatch]
    [Route("{memberId}/accept")]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<IActionResult> AcceptCourseMember([FromRoute] string memberId, [FromRoute] int courseId)
    {
        var command = new AcceptCourseMemberCommand
        {
            CourseId = courseId,
            UserId = memberId
        };
        await mediator.Send(command);

        return NoContent();
    }

    [HttpDelete]
    [Route("{memberId}")]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<IActionResult> RemoveCourseMember([FromRoute] string memberId, [FromRoute] int courseId)
    {
        var command = new RemoveCourseMemberCommand
        {
            CourseId = courseId,
            UserId = memberId
        };
        await mediator.Send(command);

        return NoContent();
    }
}