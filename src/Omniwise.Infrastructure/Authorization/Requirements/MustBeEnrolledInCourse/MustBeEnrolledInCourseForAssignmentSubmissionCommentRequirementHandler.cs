using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Omniwise.Application.Common.Types;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Extensions;
using Omniwise.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Authorization.Requirements.MustBeEnrolledInCourse;

internal class MustBeEnrolledInCourseForAssignmentSubmissionCommentRequirementHandler(OmniwiseDbContext dbContext) : AuthorizationHandler<MustBeEnrolledInCourseRequirement, AssignmentSubmissionComment>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MustBeEnrolledInCourseRequirement requirement, AssignmentSubmissionComment resource)
    {
        var currentUserId = context.User.GetUserId();

        var relatedAssignment = await dbContext.AssignmentSubmissions
            .Where(asub => asub.Id == resource.AssignmentSubmissionId)
            .Select(asub => asub.Assignment)
            .FirstAsync();

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
