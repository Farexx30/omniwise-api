using MediatR;

namespace Omniwise.Application.Lectures.Commands.CreateLecture;

public class CreateLectureCommand : IRequest<int>
{
    public required string Name { get; set; }
    public string? Content { get; set; }
    public int CourseId { get; set; }
}