using BlogGenerator.DAL;
using BlogGenerator.Interfaces;
using BlogGenerator.ServiceModels.v1.Notifications;
using Microsoft.EntityFrameworkCore;

namespace BlogGenerator.BAL.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // GET ALL NOTIFICATIONS
        // GET /api/notifications
        // =========================================================

        public async Task<List<NotificationDto>> GetNotificationsAsync(
            int userId)
        {
            return await _context.Notifications
                .Where(n => n.ReceiverUserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    NotificationId = n.NotificationId,
                    SenderUserId = n.SenderUserId,
                    BlogId = n.BlogId,
                    CommentId = n.CommentId,
                    NotificationType = n.NotificationType,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();
        }

        // =========================================================
        // MARK NOTIFICATION AS READ
        // PUT /api/notifications/{notificationId}/read
        // =========================================================

        public async Task<bool> MarkAsReadAsync(
            int notificationId,
            int userId)
        {
            var notification =
                await _context.Notifications
                    .FirstOrDefaultAsync(n =>
                        n.NotificationId == notificationId &&
                        n.ReceiverUserId == userId);

            if (notification == null)
                return false;

            notification.IsRead = true;

            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // MARK ALL NOTIFICATIONS AS READ
        // PUT /api/notifications/read-all
        // =========================================================

        public async Task<bool> MarkAllAsReadAsync(
            int userId)
        {
            var notifications =
                await _context.Notifications
                    .Where(n =>
                        n.ReceiverUserId == userId &&
                        !n.IsRead)
                    .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();

            return true;
        }
    }
}