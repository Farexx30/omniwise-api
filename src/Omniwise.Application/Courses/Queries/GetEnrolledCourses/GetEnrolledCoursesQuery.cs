using MediatR;
using Omniwise.Application.Courses.Dtos;

namespace Omniwise.Application.Courses.Queries.GetEnrolledCourses;

public class GetEnrolledCoursesQuery(string id) : IRequest<IEnumerable<CourseDto>>
{
    public string Id { get; } = id;
}
