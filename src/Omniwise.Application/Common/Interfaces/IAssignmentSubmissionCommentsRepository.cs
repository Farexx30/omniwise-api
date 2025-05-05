using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Common.Interfaces;

public interface IAssignmentSubmissionCommentsRepository
{
    Task<int> CreateAsync(AssignmentSubmissionComment comment);
    Task<AssignmentSubmissionComment?> GetByIdAsync(int assignmentSubmissionCommentId);
    Task SaveChangesAsync();
}
