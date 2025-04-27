using MediatR;
using Omniwise.Application.Assignments.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Assignments.Queries.GetAllCourseAssignments;

public class GetAllCourseAssignmentsQuery : IRequest<IEnumerable<BasicAssignmentDto>>
{
    public required int CourseId { get; init; }
}
