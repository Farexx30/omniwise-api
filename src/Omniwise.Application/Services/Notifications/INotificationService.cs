namespace Omniwise.Application.Services.Notifications;

public interface INotificationService
{
    Task NotifyUserAsync(string content, string userId);
    Task NotifyUsersAsync(string content, List<string> userIds);
}
