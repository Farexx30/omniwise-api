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
    public string? ImgBlobName { get; set; }
    public string OwnerId { get; set; } = default!;

    //References:
    public List<User> Members { get; set; } = [];
    public List<Lecture> Lectures { get; set; } = [];
    public List<Assignment> Assignments { get; set; } = [];
}