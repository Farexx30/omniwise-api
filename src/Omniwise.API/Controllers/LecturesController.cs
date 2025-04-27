using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omniwise.Application.Assignments.Dtos;
using Omniwise.Application.Lectures.Commands.CreateLecture;
using Omniwise.Application.Lectures.Queries.GetLectureById;
using Omniwise.Domain.Constants;

namespace Omniwise.API.Controllers;

[ApiController]
[Route("api/courses/{courseId}/lectures")]
[Authorize(Roles = $"{Roles.Teacher},{Roles.Student}")]
public class LecturesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<IActionResult> CreateLecture([FromRoute] int courseId, [FromBody] CreateLectureCommand command)
    {
        command.CourseId = courseId;
        int lectureId = await mediator.Send(command);
        return CreatedAtAction(nameof(GetLectureById), new { courseId, lectureId }, null);
    }


    [HttpGet("{lectureId}")]
    public async Task<ActionResult<AssignmentDto>> GetLectureById([FromRoute] int courseId, [FromRoute] int lectureId)
    {
        var lecture = await mediator.Send(new GetLectureByIdQuery(courseId, lectureId)); ;

        return Ok(lecture);
    }
}
