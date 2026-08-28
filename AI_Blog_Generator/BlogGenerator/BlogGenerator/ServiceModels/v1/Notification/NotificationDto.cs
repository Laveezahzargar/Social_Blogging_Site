using BlogGenerator.Enums;

namespace BlogGenerator.ServiceModels.v1.Notifications
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }

        public int? SenderUserId { get; set; }

        public int? BlogId { get; set; }

        public int? CommentId { get; set; }

        public NotificationType NotificationType { get; set; }

        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}