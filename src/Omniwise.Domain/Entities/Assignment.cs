using Omniwise.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Domain.Entities;

public class Assignment : ICourseResource
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Content { get; set; }
    public DateTime Deadline { get; set; }
    public float MaxGrade { get; set; }
    public int CourseId { get; set; }

    //References:
    public List<AssignmentSubmission> Submissions { get; set; } = [];
    public List<AssignmentFile> Files { get; set; } = [];
}