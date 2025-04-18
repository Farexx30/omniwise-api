using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Domain.Entities;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? ImgUrl { get; set; }
    public string OwnerId { get; set; } = default!;
}