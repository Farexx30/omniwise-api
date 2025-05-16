namespace Omniwise.Application.Services.Notifications;

public interface INotificationService
{
    Task NotifyUserAsync(string content, string userId);
}
