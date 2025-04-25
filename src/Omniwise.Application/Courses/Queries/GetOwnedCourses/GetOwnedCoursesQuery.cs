using MediatR;
using Omniwise.Application.Courses.Dtos;

namespace Omniwise.Application.Courses.Queries.GetOwnedCourses;

public class GetOwnedCoursesQuery : IRequest<IEnumerable<CourseDto>>
{
}
