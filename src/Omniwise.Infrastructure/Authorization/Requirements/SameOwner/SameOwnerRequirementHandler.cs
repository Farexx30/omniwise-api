using Microsoft.AspNetCore.Authorization;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Authorization.Requirements.SameOwner;

public class SameOwnerRequirementHandler : AuthorizationHandler<SameOwnerRequirement, Course>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameOwnerRequirement requirement, Course resource)
    {     
        var currentUserId = context.User.GetUserId();

        if (resource.OwnerId == currentUserId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
