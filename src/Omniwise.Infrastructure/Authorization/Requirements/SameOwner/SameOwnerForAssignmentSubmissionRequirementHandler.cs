using Microsoft.AspNetCore.Authorization;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Extensions;
using Omniwise.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Authorization.Requirements.SameOwner;

public class SameOwnerForAssignmentSubmissionRequirementHandler : AuthorizationHandler<SameOwnerRequirement, AssignmentSubmission>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameOwnerRequirement requirement, AssignmentSubmission resource)
    {
        var currentUserId = context.User.GetUserId();

        if (resource.AuthorId == currentUserId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
