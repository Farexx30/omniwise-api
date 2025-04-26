using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Interfaces;
using Omniwise.Infrastructure.Extensions;
using Omniwise.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Authorization.Requirements.MustBeEnrolledInCourse;

internal class MustBeEnrolledInCourseRequirementHandler(OmniwiseDbContext dbContext) : AuthorizationHandler<MustBeEnrolledInCourseRequirement, ICourseResource>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MustBeEnrolledInCourseRequirement requirement, ICourseResource resource)
    {
        var currentUserId = context.User.GetUserId();

        var isCourseMember = await dbContext.UserCourses
            .Where(uc => uc.CourseId == resource.CourseId
                   && uc.UserId == currentUserId
                   && uc.IsAccepted)
            .AnyAsync();

        if (isCourseMember)
        {
            context.Succeed(requirement);
        }
    }
}
