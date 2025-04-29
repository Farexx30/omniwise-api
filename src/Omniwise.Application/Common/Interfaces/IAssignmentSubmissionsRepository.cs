using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Common.Interfaces;

public interface IAssignmentSubmissionsRepository
{
    Task<int> CreateAsync(AssignmentSubmission assignmentSubmission);
    Task<bool> IsAlreadySubmitted(int assignmentId, string userId);
}
