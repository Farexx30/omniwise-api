using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Extensions;

internal static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal? user)
    {
        return user?.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }

    public static IEnumerable<string> GetUserRoles(this ClaimsPrincipal? user)
    {
        return user?.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value) ?? [];
    }
}
