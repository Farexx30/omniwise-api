using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Persistence.Seeders;

/// <summary>
/// UserSeeder seeds only the admin user because currently there is no need to seed other users.
/// </summary>
internal class UserSeeder(OmniwiseDbContext dbContext, 
    UserManager<User> userManager, 
    IConfiguration configuration) : ISeeder<User>
{
    public async Task SeedAsync()
    {
        if (!await dbContext.Database.CanConnectAsync())
        {
            throw new Exception("Cannot connect to the database");
        }

        if (await dbContext.Users.AnyAsync())
        {
            return;
        }

        var admin = CreateAdmin();
        var adminPassword = configuration["SeedAdmin:Password"]!;
        
        using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var userCreationResult = await userManager.CreateAsync(admin, adminPassword);
            if (!userCreationResult.Succeeded)
            {
                throw new Exception("Failed to create admin user");
            }

            var roleAssignmentResult = await userManager.AddToRoleAsync(admin, Roles.Admin);
            if (!roleAssignmentResult.Succeeded)
            {
                throw new Exception("Failed to add admin role");
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception("Couldn't create an admin user", ex);
        } 
    }

    private User CreateAdmin()
    {
        return new User
        {
            Email = configuration["SeedAdmin:Email"],
            UserName = configuration["SeedAdmin:UserName"],
            FirstName = configuration["SeedAdmin:FirstName"]!,
            LastName = configuration["SeedAdmin:LastName"]!,
            Status = UserStatus.Active
        };
    }
}
