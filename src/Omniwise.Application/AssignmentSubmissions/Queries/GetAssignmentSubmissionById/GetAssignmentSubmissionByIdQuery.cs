using MediatR;
using Omniwise.Application.AssignmentSubmissions.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.AssignmentSubmissions.Queries.GetAssignmentSubmissionById;

public class GetAssignmentSubmissionByIdQuery : IRequest<AssignmentSubmissionDto>
{
    public required int AssignmentSubmissionId { get; init; }
}
