using Microsoft.EntityFrameworkCore;
using Omniwise.Application.Common.Interfaces;
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
                  firstJoinResult => firstJoinResult.UserRole.RoleId,
                  role => role.Id,
                  (firstJoinResult, role) => new { firstJoinResult.User, Role = role })
            .Where(secondJoinResult => secondJoinResult.User.Status == status
                   && secondJoinResult.Role.NormalizedName != Roles.Admin)
            .Select(secondJoinResult => new UserWithRoleDto
            {
                Id = secondJoinResult.User.Id,
                FirstName = secondJoinResult.User.FirstName,
                LastName = secondJoinResult.User.LastName,
                Email = secondJoinResult.User.Email!,
                Status = secondJoinResult.User.Status,
                RoleName = secondJoinResult.Role.Name!
            })
            .ToListAsync();

        return userDtos;
    }

    public Task SaveChangesAsync()
        => dbContext.SaveChangesAsync();
}
