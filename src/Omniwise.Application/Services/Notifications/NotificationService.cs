using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Domain.Entities;

namespace Omniwise.Application.Services.Notifications;

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

    public async Task NotifyUsersAsync(string content, List<string> userIds)
    {
        foreach (var userId in userIds)
        {
            await NotifyUserAsync(content, userId);
        }
    }
}
