using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.AssignmentSubmissions.Dtos;

public class BasicAssignmentSubmissionDto
{
    public int Id { get; set; }
    public float? Grade { get; set; }
    public DateTime LatestSubmissionDate { get; set; }
    public string AuthorFullName { get; set; } = default!;
}
