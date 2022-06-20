namespace LogBackend.Models;

public record PushNotificationData
{
    public string NotificationToken { get; set; }
    public string BodyText { get; set; }
    public string TitleText { get; set; }
}