using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Common.Types;

public record CurrentUser
{
    public string? Id { get; init; }
    public IEnumerable<string> Roles { get; init; } = [];
    public required bool IsAuthenticated { get; init; }

    public bool IsInRole(string role)
        => Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
}