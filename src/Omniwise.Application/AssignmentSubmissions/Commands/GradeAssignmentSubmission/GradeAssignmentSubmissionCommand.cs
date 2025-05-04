using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.AssignmentSubmissions.Commands.GradeAssignmentSubmission;

public class GradeAssignmentSubmissionCommand : IRequest
{
    public float? Grade { get; init; }
    public int AssignmentSubmissionId { get; set; }
}
