using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omniwise.Application.Notifications.Commands.DeleteNotification;
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

    [HttpDelete]
    [Route("{notificationId}")]
    public async Task<IActionResult> DeleteNotification([FromRoute] int notificationId)
    {
        var command = new DeleteNotificationCommand { NotificationId = notificationId };
        await mediator.Send(command);
        return NoContent();
    }

}
