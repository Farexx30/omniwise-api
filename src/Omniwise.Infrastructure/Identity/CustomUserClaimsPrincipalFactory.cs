using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Identity;

public class CustomUserClaimsPrincipalFactory(UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<IdentityOptions> options) : UserClaimsPrincipalFactory<User, IdentityRole>(userManager, roleManager, options)
{
    public override async Task<ClaimsPrincipal> CreateAsync(User user)
    {
        var claimsIdentity = await GenerateClaimsAsync(user);

        var firstNameClaim = new Claim(CustomClaimTypes.FirstName, user.FirstName);
        var lastNameClaim = new Claim(CustomClaimTypes.LastName, user.LastName);

        claimsIdentity.AddClaims([firstNameClaim, lastNameClaim]);

        return new ClaimsPrincipal(claimsIdentity);
    }
}
