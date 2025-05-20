using MediatR;

namespace Omniwise.Application.Notifications.Commands.DeleteNotification;

public class DeleteNotificationCommand : IRequest
{
    public int NotificationId { get; set; }
}
