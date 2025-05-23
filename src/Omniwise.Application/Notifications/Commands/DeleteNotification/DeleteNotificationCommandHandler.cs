using MediatR;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces.Identity;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Domain.Exceptions;

namespace Omniwise.Application.Notifications.Commands.DeleteNotification;

public class DeleteNotificationCommandHandler(ILogger<DeleteNotificationCommandHandler> logger,
    INotificationsRepository notificationsRepository,
    IUserContext userContext) : IRequestHandler<DeleteNotificationCommand>
{
    public async Task Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var notificationId = request.NotificationId;
        var userId = userContext.GetCurrentUser().Id!;

        var notification = await notificationsRepository.GetByIdAsync(notificationId, userId)
            ?? throw new NotFoundException($"Notification with id = {notificationId} not found.");

        logger.LogInformation("Deleting notification with id: {NotificationId} from the repository.", notificationId);
                
        await notificationsRepository.DeleteNotificationAsync(notification);
    }
}
