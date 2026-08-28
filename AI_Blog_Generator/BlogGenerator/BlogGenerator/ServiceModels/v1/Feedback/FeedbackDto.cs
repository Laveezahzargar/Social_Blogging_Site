using BlogGenerator.Enums;

namespace BlogGenerator.ServiceModels.v1.Feedback
{
    public class FeedbackDto
    {
        public int FeedbackId { get; set; }

        public int UserId { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public int Rating { get; set; }

        public bool IsPublic { get; set; }

        public FeedbackStatus Status { get; set; }

        public string? AdminResponse { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}