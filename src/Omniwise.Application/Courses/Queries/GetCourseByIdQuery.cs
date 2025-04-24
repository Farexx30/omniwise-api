using MediatR;
using Omniwise.Application.Courses.Dtos;

namespace Omniwise.Application.Courses.Queries;

public class GetCourseByIdQuery(int id) : IRequest<CourseDto>
{
    public int Id { get; set; } = id;
}

