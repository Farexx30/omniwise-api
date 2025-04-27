using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omniwise.Application.Assignments.Dtos;
using Omniwise.Application.Lectures.Queries.GetLectureById;
using Omniwise.Domain.Constants;

namespace Omniwise.API.Controllers;

[ApiController]
[Route("api/courses/{courseId}/lectures")]
[Authorize(Roles = $"{Roles.Teacher},{Roles.Student}")]
public class LecturesController(IMediator mediator) : ControllerBase
{
    [HttpGet("{lectureId}")]
    public async Task<ActionResult<AssignmentDto>> GetLectureById([FromRoute] int courseId, [FromRoute] int lectureId)
    {
        var lecture = await mediator.Send(new GetLectureByIdQuery(courseId, lectureId)); ;

        return Ok(lecture);
    }
}
