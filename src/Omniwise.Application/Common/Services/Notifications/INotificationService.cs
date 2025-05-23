namespace Omniwise.Application.Common.Services.Notifications;

public interface INotificationService
{
    Task NotifyUserAsync(string content, string userId);
    Task NotifyUsersAsync(string content, IEnumerable<string> userIds);
}
