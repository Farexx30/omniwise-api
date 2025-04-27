namespace Omniwise.Application.Lectures.Dtos;

public class LectureDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Content { get; set; }
    public int CourseId { get; set; }
}
