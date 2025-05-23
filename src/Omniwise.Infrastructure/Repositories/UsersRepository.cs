using Microsoft.EntityFrameworkCore;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Application.Users.Dtos;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Infrastructure.Repositories;

internal class UsersRepository(OmniwiseDbContext dbContext) : IUsersRepository
{
    public async Task<User?> GetByIdAsync(string userId)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user;
    }

    public async Task DeleteAsync(User user)
    {
        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<UserWithRoleDto>> GetAllByStatusAsync(UserStatus status)
    {
        var userDtos = await dbContext.Users
            .Join(dbContext.UserRoles,
                  user => user.Id,
                  userRole => userRole.UserId,
                  (user, userRole) => new { User = user, UserRole = userRole })
            .Join(dbContext.Roles,
                  currentResult => currentResult.UserRole.RoleId,
                  role => role.Id,
                  (currentResult, role) => new { currentResult.User, Role = role })
            .Where(currentResult => currentResult.User.Status == status
                   && currentResult.Role.Name != Roles.Admin)
            .Select(finalResult => new UserWithRoleDto
            {
                Id = finalResult.User.Id,
                FirstName = finalResult.User.FirstName,
                LastName = finalResult.User.LastName,
                Email = finalResult.User.Email!,
                Status = finalResult.User.Status,
                RoleName = finalResult.Role.Name!
            })
            .ToListAsync();

        return userDtos;
    }

    public Task SaveChangesAsync()
        => dbContext.SaveChangesAsync();
}
