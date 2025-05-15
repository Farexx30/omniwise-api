using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Notifications.Dtos;

namespace Omniwise.Application.Notifications.Queries.GetNotifications;

public class GetNotificationsQueryHandler(ILogger<GetNotificationsQueryHandler> logger,
    IMapper mapper,
    INotificationsRepository notificationRepository,
    IUserContext userContext) : IRequestHandler<GetNotificationsQuery, IEnumerable<NotificationDto>>
{
    public async Task<IEnumerable<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetCurrentUser().Id!;

        logger.LogInformation("Fetching all notifications for user with id: {UserId} from the repository.", userId);

        var notifications = await notificationRepository.GetAllNotificationsAsync(userId);
        var notificationsDtos = mapper.Map<IEnumerable<NotificationDto>>(notifications);

        return notificationsDtos;
    }
}
