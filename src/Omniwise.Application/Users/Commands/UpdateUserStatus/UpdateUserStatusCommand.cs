using MediatR;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Users.Commands.UpdateUserStatus;

public class UpdateUserStatusCommand : IRequest
{
    public required UserStatus Status { get; init; }
    public string UserId { get; set; } = string.Empty;
}
