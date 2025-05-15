using Omniwise.Domain.Entities;

namespace Omniwise.Application.Common.Interfaces;

public interface INotificationsRepository
{
    Task<IEnumerable<Notification>> GetAllNotificationsAsync(string userId);
}
