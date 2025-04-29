using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Extensions;
using Omniwise.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Authorization.Requirements.MustBeEnrolledInCourse;

internal class MustBeEnrolledInCourseForAssignmentSubmissionRequirementHandler(OmniwiseDbContext dbContext) : AuthorizationHandler<MustBeEnrolledInCourseRequirement, AssignmentSubmission>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MustBeEnrolledInCourseRequirement requirement, AssignmentSubmission resource)
    {
        var currentUserId = context.User.GetUserId();

        var relatedAssignment = await dbContext.Assignments
            .FirstAsync(asub => asub.Id == resource.AssignmentId);

        var isCourseMember = await dbContext.UserCourses
            .Where(uc => uc.CourseId == relatedAssignment.CourseId
                   && uc.UserId == currentUserId
                   && uc.IsAccepted)
            .AnyAsync();

        if (isCourseMember)
        {
            context.Succeed(requirement);
        }
    }
}
