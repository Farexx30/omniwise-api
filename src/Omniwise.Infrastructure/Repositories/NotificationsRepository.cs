using Microsoft.EntityFrameworkCore;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Persistence;

namespace Omniwise.Infrastructure.Repositories;

internal class NotificationsRepository(OmniwiseDbContext dbContext) : INotificationsRepository
{
   public async Task<IEnumerable<Notification>> GetAllNotificationsAsync(string userId)
    {
        var notifications = await dbContext.Notifications
           .Where(n => n.UserId == userId)
           .ToListAsync();

        return notifications;
    }

    public async Task<Notification?> GetByIdAsync(int notificationId, string userId)
    {
        var notification = await dbContext.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        return notification;
    }

    public async Task DeleteNotificationAsync(Notification notification)
    {
        dbContext.Notifications.Remove(notification);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddNotificationAsync(Notification notification)
    {
        dbContext.Notifications.Add(notification);
        await dbContext.SaveChangesAsync();
    }

}
