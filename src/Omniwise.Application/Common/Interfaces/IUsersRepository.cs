using Omniwise.Application.Users.Dtos;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Common.Interfaces;

public interface IUsersRepository
{
    Task<IEnumerable<UserWithRoleDto>> GetAllByStatusAsync(UserStatus status);
}
