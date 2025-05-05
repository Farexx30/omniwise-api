using Microsoft.AspNetCore.Authorization;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Authorization.Requirements.SameOwner;

public class SameOwnerForAssignmentSubmissionCommentRequirementHandler : AuthorizationHandler<SameOwnerRequirement, AssignmentSubmissionComment>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameOwnerRequirement requirement, AssignmentSubmissionComment resource)
    {
        var currentUserId = context.User.GetUserId();

        if (resource.AuthorId == currentUserId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
