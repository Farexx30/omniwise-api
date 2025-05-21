using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Assignments.Commands.UpdateAssignment;

public class UpdateAssignmentCommand : IRequest
{
    public int AssignmentId { get; set; }
    public required string Name { get; init; }
    public string? Content { get; init; }
    public required DateTime Deadline { get; init; }
    public required float MaxGrade { get; init; }
    public IEnumerable<IFormFile> Files { get; set; } = [];
}
