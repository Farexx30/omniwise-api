using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omniwise.Application.Notifications.Queries.GetNotifications;
using Omniwise.Domain.Constants;

namespace Omniwise.API.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize(Roles = $"{Roles.Teacher},{Roles.Student}")]
public class NotificationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var notifications = await mediator.Send(new GetNotificationsQuery());
        return Ok(notifications);
    }


}
