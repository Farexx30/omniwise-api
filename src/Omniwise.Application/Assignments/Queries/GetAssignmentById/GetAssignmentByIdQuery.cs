using MediatR;
using Omniwise.Application.Assignments.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Assignments.Queries.GetAssignmentById;

public class GetAssignmentByIdQuery : IRequest<AssignmentDto>
{
    public required int AssignmentId { get; init; }
    public required int CourseId { get; init; }
}
