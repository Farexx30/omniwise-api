using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Courses.Commands.CreateCourse;

public class CreateCourseCommand : IRequest<int>
{
    public required string Name { get; init; }
    public string? ImgUrl { get; init; }
}
