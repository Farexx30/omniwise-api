using Microsoft.AspNetCore.Identity;
using Omniwise.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Seeders;

internal class RoleSeeder(RoleManager<IdentityRole> roleManager) : ISeeder<IdentityRole>
{
    private readonly IEnumerable<string> _roleNames = [Roles.Admin, 
        Roles.Teacher, 
        Roles.Student];

    public async Task SeedAsync()
    {
        foreach (var roleName in _roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = CreateRole(roleName);
                await roleManager.CreateAsync(role);
            }
        }
    }

    private static IdentityRole CreateRole(string name)
    {
        return new IdentityRole
        {
            Name = name,
            NormalizedName = name.ToUpper()
        };
    }  
}
