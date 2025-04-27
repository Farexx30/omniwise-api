using Omniwise.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Domain.Entities;

public class Lecture : ICourseResource
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Content { get; set; }
    public int CourseId { get; set; }

    //References:
    public List<LectureFile> Files { get; set; } = [];
}