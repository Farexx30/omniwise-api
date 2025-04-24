using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Courses.Commands.DeleteCourse
{
    public class DeleteCourseCommand : IRequest
    {
        public required int Id { get; init; }
    }
}
