using MediatR;
using Omniwise.Application.Courses.Dtos;

namespace Omniwise.Application.Courses.Queries.GetCourseById;

public class GetCourseByIdQuery(int id) : IRequest<CourseDto>
{
    public int Id { get; } = id;
}

