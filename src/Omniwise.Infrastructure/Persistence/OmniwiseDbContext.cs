using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Omniwise.Domain.Entities.File;

namespace Omniwise.Infrastructure.Persistence;

internal class OmniwiseDbContext(DbContextOptions<OmniwiseDbContext> options)
    : IdentityDbContext<User>(options)
{
    internal DbSet<Course> Courses { get; set; }
    internal DbSet<UserCourse> UserCourses { get; set; }
    internal DbSet<Lecture> Lectures { get; set; }
    internal DbSet<Assignment> Assignments { get; set; }
    internal DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }
    internal DbSet<AssignmentSubmissionComment> AssignmentSubmissionComments { get; set; }
    internal DbSet<File> Files { get; set; }
    internal DbSet<LectureFile> LectureFiles { get; set; }
    internal DbSet<AssignmentFile> AssignmentFiles { get; set; }
    internal DbSet<AssignmentSubmissionFile> AssignmentSubmissionFiles { get; set; }
    internal DbSet<Notification> Notifications { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //Apply all configurations from THIS assembly:
        builder.ApplyConfigurationsFromAssembly(typeof(OmniwiseDbContext).Assembly);
    }
}
