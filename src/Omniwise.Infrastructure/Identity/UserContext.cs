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
    public ClaimsPrincipal? ClaimsPrincipalUser
    {
        get => httpContextAccessor?.HttpContext?.User;
    }

    public CurrentUser GetCurrentUser()
    {
        var user = ClaimsPrincipalUser 
            ?? throw new InvalidOperationException("User context is not present");

        if (user.Identity is null || !user.Identity.IsAuthenticated)
        {
            return CreateCurrentUser(id: null, 
                roles: [], 
                firstName: null,
                lastName: null);
        }

        var userId = user.GetUserId();
        var userRoles = user.GetUserRoles();
        var userFirstName = user.GetUserFirstName();
        var userLastName = user.GetUserLastName();

        return CreateCurrentUser(id: userId,
                roles: userRoles,
                firstName: userFirstName,
                lastName: userLastName);
    }

    private static CurrentUser CreateCurrentUser(string? id, IEnumerable<string> roles, string? firstName, string? lastName)
    {
        return new CurrentUser
        {
            Id = id,
            Roles = roles,
            FirstName = firstName,
            LastName = lastName
        };
    }
}
