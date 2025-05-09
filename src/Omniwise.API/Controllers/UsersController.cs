using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omniwise.Application.Users.Dtos;
using Omniwise.Application.Users.Queries.GetAllUsersByStatus;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;

namespace Omniwise.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = Roles.Admin)]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<UserWithRoleDto>> GetAllUsersByStatus([FromQuery] GetAllUsersByStatusCommand command)
    {
        var users = await mediator.Send(command);

        return Ok(users);
    }
}
