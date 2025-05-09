using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omniwise.Application.Users.Commands.UpdateUserStatus;
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
    [HttpPatch("{userId}/status")]
    public async Task<IActionResult> UpdateUserStatus([FromBody] UpdateUserStatusCommand command, [FromRoute] string userId)
    {
        command.UserId = userId;
        await mediator.Send(command);

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<UserWithRoleDto>> GetAllUsersByStatus([FromQuery] GetAllUsersByStatusQuery query)
    {
        var users = await mediator.Send(query);

        return Ok(users);
    }
}
