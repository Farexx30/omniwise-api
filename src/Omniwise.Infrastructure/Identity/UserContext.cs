using Microsoft.AspNetCore.Http;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Common.Types;
using Omniwise.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Identity;

internal class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public CurrentUser GetCurrentUser()
    {
        var user = (httpContextAccessor?.HttpContext?.User) 
            ?? throw new InvalidOperationException("User context is not present");

        if (user.Identity is null || !user.Identity.IsAuthenticated)
        {
            return CreateCurrentUser(id: null, 
                roles: [], 
                isAuthenticated: false);
        }

        var userId = user.GetUserId();
        var userRoles = user.GetUserRoles();

        return CreateCurrentUser(id: userId,
                roles: userRoles,
                isAuthenticated: true);
    }

    private static CurrentUser CreateCurrentUser(string? id, IEnumerable<string> roles, bool isAuthenticated)
    {
        return new CurrentUser
        {
            Id = id,
            Roles = roles,
            IsAuthenticated = isAuthenticated
        };
    }
}
