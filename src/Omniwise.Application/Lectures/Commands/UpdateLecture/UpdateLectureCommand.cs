using MediatR;
using Microsoft.AspNetCore.Http;

namespace Omniwise.Application.Lectures.Commands.UpdateLecture;

public class UpdateLectureCommand : IRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Content { get; set; }
    public IEnumerable<IFormFile> Files { get; init; } = [];
}
