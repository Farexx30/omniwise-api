using Omniwise.Application.Common.Types;
using System.Security.Claims;

namespace Omniwise.Application.Common.Interfaces;

public interface IUserContext
{
    ClaimsPrincipal? ClaimsPrincipalUser { get; }
    CurrentUser GetCurrentUser();
}
