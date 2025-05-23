using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Domain.Entities;

namespace Omniwise.Application.Common.Services.Notifications;

internal class NotificationService(INotificationsRepository notificationsRepository) : INotificationService
{
    public async Task NotifyUserAsync(string content, string userId)
    {
        var notification = new Notification
        {
            Content = content,
            SentDate = DateTime.UtcNow,
            UserId = userId
        };

        await notificationsRepository.AddNotificationAsync(notification);
    }

    public async Task NotifyUsersAsync(string content, IEnumerable<string> userIds)
    {
        List<Notification> notifications = [];
        foreach (var userId in userIds)
        {
            notifications.Add(new Notification
            {
                Content = content,
                SentDate = DateTime.UtcNow,
                UserId = userId
            });
        }

        await notificationsRepository.AddNotificationsAsync(notifications);
    }
}
