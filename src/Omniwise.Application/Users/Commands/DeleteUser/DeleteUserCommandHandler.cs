using MediatR;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(IUsersRepository usersRepository,
    ILogger<DeleteUserCommandHandler> logger) : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;

        var user = await usersRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException($"User with id = {userId} not found.");

        if (user.Status == UserStatus.Active
            || user.Status == UserStatus.Archived)
        {
            logger.LogInformation("User with id = {UserId} cannot be deleted due to its status '{user.Status}'.", 
                userId,
                user.Status);

            throw new ForbiddenException($"User with id = {userId} cannot be deleted due to its status.");
        }

        logger.LogInformation("Deleting user with id = {UserId}.", userId);

        await usersRepository.DeleteAsync(user);
    }
}
