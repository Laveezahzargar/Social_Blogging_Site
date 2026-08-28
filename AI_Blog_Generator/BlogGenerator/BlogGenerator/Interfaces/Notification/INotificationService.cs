using BlogGenerator.ServiceModels.v1.Notifications;

namespace BlogGenerator.Interfaces
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetNotificationsAsync(
            int userId);

        Task<bool> MarkAsReadAsync(
            int notificationId,
            int userId);

        Task<bool> MarkAllAsReadAsync(
            int userId);
    }
}