using BlogGenerator.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogGenerator.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationsController(
            INotificationService service)
        {
            _service = service;
        }

        private int CurrentUserId =>
            int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!.Value);

        // =========================================================
        // GET ALL NOTIFICATIONS
        // GET /api/notifications
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var result =
                await _service.GetNotificationsAsync(
                    CurrentUserId);

            return Ok(result);
        }

        // =========================================================
        // MARK NOTIFICATION AS READ
        // PUT /api/notifications/{notificationId}/read
        // =========================================================

        [HttpPut("{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(
            int notificationId)
        {
            var result =
                await _service.MarkAsReadAsync(
                    notificationId,
                    CurrentUserId);

            if (!result)
                return NotFound(
                    "Notification not found.");

            return Ok(new
            {
                success = true,
                message = "Notification marked as read."
            });
        }

        // =========================================================
        // MARK ALL AS READ
        // PUT /api/notifications/read-all
        // =========================================================

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _service.MarkAllAsReadAsync(
                CurrentUserId);

            return Ok(new
            {
                success = true,
                message = "All notifications marked as read."
            });
        }
    }
}