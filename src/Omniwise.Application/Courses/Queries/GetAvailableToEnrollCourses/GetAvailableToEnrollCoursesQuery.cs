using MediatR;
using Omniwise.Application.Courses.Dtos;

namespace Omniwise.Application.Courses.Queries.GetAvailableToEnrollCourses;

public class GetAvailableToEnrollCoursesQuery : IRequest<IEnumerable<CourseDto>>
{
    public string? SearchPhrase { get; set; }
    public string Id { get; set; } = string.Empty;
}
