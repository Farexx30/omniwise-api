using MediatR;

namespace Omniwise.Application.Lectures.Commands.DeleteLecture;

public class DeleteLectureCommand : IRequest
{
    public required int LectureId { get; init; }
}
