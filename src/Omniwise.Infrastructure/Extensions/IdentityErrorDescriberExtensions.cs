using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Extensions;

public static class IdentityErrorDescriberExtensions
{
    public static IdentityError InvalidFirstName(this IdentityErrorDescriber _, string firstName)
    {
        return new IdentityError
        {
            Code = nameof(InvalidFirstName),
            Description = $"FirstName '{firstName}' is invalid - make sure it is not empty."
        };
    }

    public static IdentityError InvalidLastName(this IdentityErrorDescriber _, string lastName)
    {
        return new IdentityError
        {
            Code = nameof(InvalidLastName),
            Description = $"LastName '{lastName}' is invalid - make sure it is not empty."
        };
    }
}
