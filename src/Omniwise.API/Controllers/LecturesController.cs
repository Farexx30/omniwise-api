using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omniwise.Application.Assignments.Dtos;
using Omniwise.Application.Lectures.Commands.CreateLecture;
using Omniwise.Application.Lectures.Commands.DeleteLecture;
using Omniwise.Application.Lectures.Commands.UpdateLecture;
using Omniwise.Application.Lectures.Queries.GetLectureById;
using Omniwise.Application.Lectures.Queries.GetLectures;
using Omniwise.Domain.Constants;

namespace Omniwise.API.Controllers;

[ApiController]
[Route("api/courses/{courseId}/lectures")]
[Authorize(Roles = $"{Roles.Teacher},{Roles.Student}")]
public class LecturesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = Roles.Teacher)]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> CreateLecture([FromForm] CreateLectureCommand command, [FromRoute] int courseId)
    {
        command.CourseId = courseId;
        int lectureId = await mediator.Send(command);
        return CreatedAtAction(nameof(GetLectureById), new { courseId, lectureId }, null);
    }

    [HttpPatch("{lectureId}")]
    [Authorize(Roles = Roles.Teacher)]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> UpdateLecture([FromForm] UpdateLectureCommand command, [FromRoute] int courseId, [FromRoute] int lectureId)
    {
        command.CourseId = courseId;
        command.Id = lectureId;
        await mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{lectureId}")]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<IActionResult> DeleteLecture([FromRoute] int courseId, [FromRoute] int lectureId)
    {
        var command = new DeleteLectureCommand { CourseId = courseId, Id = lectureId };
        await mediator.Send(command);
        return NoContent();
    }


    [HttpGet("{lectureId}")]
    public async Task<ActionResult<AssignmentDto>> GetLectureById([FromRoute] int courseId, [FromRoute] int lectureId)
    {
        var lecture = await mediator.Send(new GetLectureByIdQuery(courseId, lectureId)); ;

        return Ok(lecture);
    }

    [HttpGet]
    public async Task<IActionResult> GetLectures([FromRoute] int courseId)
    {
        var lectures = await mediator.Send(new GetLecturesQuery(courseId));
        return Ok(lectures);
    }
}
