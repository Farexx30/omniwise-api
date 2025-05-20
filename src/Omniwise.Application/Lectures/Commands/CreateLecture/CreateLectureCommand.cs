using MediatR;
using Microsoft.AspNetCore.Http;

namespace Omniwise.Application.Lectures.Commands.CreateLecture;

public class CreateLectureCommand : IRequest<int>
{
    public required string Name { get; set; }
    public string? Content { get; set; }
    public IEnumerable<IFormFile> Files { get; init; } = [];
    public int CourseId { get; set; }
}