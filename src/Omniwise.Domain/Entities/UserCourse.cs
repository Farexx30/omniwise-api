using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Domain.Entities;

public class UserCourse
{
    public string UserId { get; set; } = default!;
    public int CourseId { get; set; }
    public DateOnly? JoinDate { get; set; }
    public bool IsAccepted { get; set; }
}
