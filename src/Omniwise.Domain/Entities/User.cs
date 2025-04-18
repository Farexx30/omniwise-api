using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Domain.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public bool IsAccepted { get; set; }

    //References:
    public List<Course> Courses { get; set; } = [];
    public List<Notification> Notifications { get; set; } = [];
}
