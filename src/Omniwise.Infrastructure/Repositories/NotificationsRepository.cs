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

}
