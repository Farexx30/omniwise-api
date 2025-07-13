using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omniwise.Application.Assignments.Dtos;
using Omniwise.Application.Assignments.Queries.GetAllCourseAssignments;
using Omniwise.Application.Lectures.Commands.CreateLecture;
using Omniwise.Application.Lectures.Commands.DeleteLecture;
using Omniwise.Application.Lectures.Commands.UpdateLecture;
using Omniwise.Application.Lectures.Queries.GetLectureById;
using Omniwise.Application.Lectures.Queries.GetAllCourseLectures;
using Omniwise.Domain.Constants;
using Omniwise.Application.Lectures.Dtos;

namespace Omniwise.API.Controllers;

[ApiController]
[Route("api")]
[Authorize(Roles = $"{Roles.Teacher},{Roles.Student}")]
public class LecturesController(IMediator mediator) : ControllerBase
{
    [HttpPost("courses/{courseId}/lectures")]
    [Authorize(Roles = Roles.Teacher)]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> CreateLecture([FromForm] CreateLectureCommand command, [FromRoute] int courseId)
    {
        command.CourseId = courseId;
        int lectureId = await mediator.Send(command);

        return CreatedAtAction(nameof(GetLectureById), new { lectureId }, new { lectureId });
    }

    [HttpPatch("lectures/{lectureId}")]
    [Authorize(Roles = Roles.Teacher)]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> UpdateLecture([FromForm] UpdateLectureCommand command, [FromRoute] int lectureId)
    {
        command.Id = lectureId;
        await mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("lectures/{lectureId}")]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<IActionResult> DeleteLecture([FromRoute] int lectureId)
    {
        var command = new DeleteLectureCommand { LectureId = lectureId };
        await mediator.Send(command);

        return NoContent();
    }

    [HttpGet("lectures/{lectureId}")]
    public async Task<ActionResult<LectureDto>> GetLectureById([FromRoute] int lectureId)
    {
        var query = new GetLectureByIdQuery { LectureId = lectureId };
        var lecture = await mediator.Send(query);

        return Ok(lecture);
    }

    [HttpGet("courses/{courseId}/lectures")]
    public async Task<ActionResult<IEnumerable<LectureToGetAllDto>>> GetAllCourseLectures([FromRoute] int courseId)
    {
        var query = new GetAllCourseLecturesQuery { CourseId = courseId };
        var lectures = await mediator.Send(query);

        return Ok(lectures);
    }
}
