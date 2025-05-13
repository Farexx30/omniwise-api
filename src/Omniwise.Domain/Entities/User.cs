using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Domain.Entities;

public enum UserStatus
{
    Pending = 0,
    Active = 1,
    Archived = 2
}

public class User : IdentityUser
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public UserStatus Status { get; set; } = UserStatus.Pending;

    //References:
    public List<UserCourse> UserCourses { get; set; } = [];
    public List<Course> OwnedCourses { get; set; } = [];
    public List<Course> EnrolledCourses { get; set; } = [];
    public List<AssignmentSubmission> AssignmentSubmissions { get; set; } = [];
    public List<AssignmentSubmissionComment> AssignmentSubmissionComments { get; set; } = [];
    public List<Notification> Notifications { get; set; } = [];
}
