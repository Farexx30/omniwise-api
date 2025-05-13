using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Users.Commands.DeleteUser;

public class DeleteUserCommand : IRequest
{
    public required string UserId { get; init; }
}
