using MediatR;

namespace Omniwise.Application.Lectures.Commands.DeleteLecture;

public class DeleteLectureCommand : IRequest
{
    public int CourseId { get; set; }
    public required int Id { get; init; }
}
