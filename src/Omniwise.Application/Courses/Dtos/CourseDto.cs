using Omniwise.Domain.Entities;

namespace Omniwise.Application.Courses.Dtos;

public class CourseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? ImgUrl { get; set; }
    public string? ImgName { get; set; }
    public string OwnerId { get; set; } = default!;
}
