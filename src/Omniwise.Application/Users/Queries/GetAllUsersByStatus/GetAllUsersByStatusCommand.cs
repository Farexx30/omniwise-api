using MediatR;
using Omniwise.Application.Users.Dtos;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Users.Queries.GetAllUsersByStatus;

public class GetAllUsersByStatusCommand : IRequest<IEnumerable<UserWithRoleDto>>
{
    public required UserStatus Status { get; init; }
}
