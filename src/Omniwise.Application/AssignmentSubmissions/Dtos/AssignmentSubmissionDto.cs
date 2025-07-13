using Omniwise.Application.AssignmentSubmissionComments.Dtos;
using Omniwise.Application.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.AssignmentSubmissions.Dtos;

public class AssignmentSubmissionDto
{
    public int Id { get; set; }
    public float? Grade { get; set; }
    public DateTime LatestSubmissionDate { get; set; }
    public string AuthorFullName { get; set; } = default!;
    public int AssignmentId { get; set; }
    public string AssignmentName { get; set; } = default!;
    public float MaxGrade { get; set; }
    public DateTime Deadline { get; set; }
    public List<FileInfoDto> FileInfos { get; set; } = [];
    public List<AssignmentSubmissionCommentDto> Comments { get; set; } = [];
}
