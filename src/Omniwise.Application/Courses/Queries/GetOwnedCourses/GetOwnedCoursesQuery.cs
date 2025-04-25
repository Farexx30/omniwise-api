using MediatR;
using Omniwise.Application.Courses.Dtos;

namespace Omniwise.Application.Courses.Queries.GetOwnedCourses;

public class GetOwnedCoursesQuery(string id) : IRequest<IEnumerable<CourseDto>>
{
    public string Id { get; } = id;
}
