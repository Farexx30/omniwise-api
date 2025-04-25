using MediatR;
using Omniwise.Application.Courses.Dtos;

namespace Omniwise.Application.Courses.Queries.GetEnrolledCourses;

public class GetEnrolledCoursesQuery : IRequest<IEnumerable<CourseDto>>
{
}
