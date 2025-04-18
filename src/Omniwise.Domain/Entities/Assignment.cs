using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Domain.Entities;

public class Assignment
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Content { get; set; }
    public DateTime Deadline { get; set; }
    public float MaxGrade { get; set; }

    public int CourseId { get; set; }
    public Course Course { get; set; } = default!;
}