using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Users.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Users.Queries.GetAllUsersByStatus;

public class GetAllUsersByStatusQueryHandler(IUsersRepository usersRepository,
    ILogger<GetAllUsersByStatusQueryHandler> logger) : IRequestHandler<GetAllUsersByStatusQuery, IEnumerable<UserWithRoleDto>>
{
    public async Task<IEnumerable<UserWithRoleDto>> Handle(GetAllUsersByStatusQuery request, CancellationToken cancellationToken)
    {
        var status = request.Status;

        logger.LogInformation("Getting all users by status: {Status}", status);

        var userDtos = await usersRepository.GetAllByStatusAsync(status);
        return userDtos;
    }
}
