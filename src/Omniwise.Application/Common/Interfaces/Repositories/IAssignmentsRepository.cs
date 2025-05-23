using Omniwise.Application.Common.Types;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Common.Interfaces.Repositories;

public interface IAssignmentsRepository
{
    Task<int> CreateAsync(Assignment assignment);
    Task DeleteAsync(Assignment assignment);
    Task<Assignment?> GetByIdAsync(int assignmentId, CurrentUser? currentUser = default, bool includeAssignmentSubmissions = false);
    Task<IEnumerable<int>> GetAllIdsByCourseIdAsync(int courseId);
    Task<IEnumerable<Assignment>> GetAllCourseAssignmentsAsync(int courseId);
    Task<float> GetMaxGradeAsync(int assignmentId);
    Task<List<string>> GetAssigmentSubmissionAuthorIds(int assignmentId);
    Task<bool> ExistsAsync(int assignmentId);
    Task SaveChangesAsync();
}
