using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Identity.Requests;

/// <summary>
/// The request type for the "/register" endpoint added by <see cref="IdentityApiEndpointRouteBuilderExtensions.MapOmniwiseIdentityApi"/>.
/// </summary>
public sealed class OmniwiseRegisterRequest
{
    //Properties from the original RegisterRequest class:
    public required string Email { get; init; }
    public required string Password { get; init; }

    //Custom properties:
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string RoleName { get; init; }
}
