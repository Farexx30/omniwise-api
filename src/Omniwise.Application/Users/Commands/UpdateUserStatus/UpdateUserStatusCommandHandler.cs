using MediatR;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Users.Queries.GetAllUsersByStatus;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Users.Commands.UpdateUserStatus;

public class UpdateUserStatusCommandHandler(IUsersRepository usersRepository,
    ILogger<UpdateUserStatusCommandHandler> logger) : IRequestHandler<UpdateUserStatusCommand>
{
    public async Task Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
    {
        var status = request.Status;
        var userId = request.UserId;

        var user = await usersRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException($"User with id = {userId} not found.");

        logger.LogInformation("Updating user status to {Status} for user with id = {UserId}", 
            status, 
            userId);

        //We don't use automapper here for readability:
        user.Status = status;
        await usersRepository.SaveChangesAsync();
    }
}
