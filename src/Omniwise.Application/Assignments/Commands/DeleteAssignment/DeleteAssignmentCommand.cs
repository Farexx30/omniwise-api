using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Assignments.Commands.DeleteAssignment;

public class DeleteAssignmentCommand : IRequest
{
    public required int AssignmentId { get; init; }
    public required int CourseId { get; init; }
}
