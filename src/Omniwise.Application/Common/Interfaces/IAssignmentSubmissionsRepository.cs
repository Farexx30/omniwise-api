using Omniwise.Application.AssignmentSubmissionComments.Dtos;
using Omniwise.Application.AssignmentSubmissions.Dtos;
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
    Task DeleteAsync(AssignmentSubmission assignmentSubmission);
    Task DeleteByAuthorIdAsync(string authorId);
    Task<AssignmentSubmission?> GetByIdAsync(int assignmentSubmissionId);
    Task<IEnumerable<int>> GetAllIdsByAssignmentIdAsync(int assignmentId);
    Task<IEnumerable<int>> GetAllIdsByAssignmentIdsAsync(IEnumerable<int> assignmentIds);
    Task<IEnumerable<int>> GetAllIdsByAuthorIdAsync(string authorId);
    Task<bool> IsAlreadySubmitted(int assignmentSubmissionId, string userId);
    Task<bool> ExistsAsync(int assignmentSubmissionId);
    Task SaveChangesAsync();
    Task<AssignmentSubmissionNotificationDetailsDto?> GetAssignmentAndCourseNames(int assignmentSubmissionId);
}
