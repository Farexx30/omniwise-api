using Omniwise.Domain.Entities;

namespace Omniwise.Application.Common.Interfaces.Repositories;

public interface INotificationsRepository
{
    Task<IEnumerable<Notification>> GetAllNotificationsAsync(string userId);
    Task<Notification?> GetByIdAsync(int notificationId, string userId);
    Task DeleteNotificationAsync(Notification notification);
    Task AddNotificationAsync(Notification notification);
    Task AddNotificationsAsync(IEnumerable<Notification> notifications);
}
