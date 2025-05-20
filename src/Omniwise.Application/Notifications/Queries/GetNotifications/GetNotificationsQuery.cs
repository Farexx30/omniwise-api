using MediatR;
using Omniwise.Application.Notifications.Dtos;

namespace Omniwise.Application.Notifications.Queries.GetNotifications;

public class GetNotificationsQuery : IRequest<IEnumerable<NotificationDto>>
{
}
