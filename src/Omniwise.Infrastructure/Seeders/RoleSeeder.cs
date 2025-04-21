using Microsoft.AspNetCore.Identity;
using Omniwise.Domain.Constants;
using Omniwise.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Seeders;

internal class RoleSeeder(OmniwiseDbContext dbContext,
    RoleManager<IdentityRole> roleManager) : ISeeder<IdentityRole>
{
    private readonly IEnumerable<string> _roleNames = [Roles.Admin, 
        Roles.Teacher, 
        Roles.Student];

    public async Task SeedAsync()
    {
        if (!await dbContext.Database.CanConnectAsync())
        {
            throw new Exception("Cannot connect to the database");
        }

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
