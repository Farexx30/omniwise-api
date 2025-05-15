namespace Omniwise.Application.Notifications.Dtos;

public class NotificationDto
{
    public int Id { get; set; }
    public string Content { get; set; } = default!;
    public DateTime SentDate { get; set; }
    public string UserId { get; set; } = default!;
}
