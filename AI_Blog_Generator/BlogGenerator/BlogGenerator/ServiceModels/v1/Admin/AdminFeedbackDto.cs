

namespace BlogGenerator.ServiceModels.v1;


public class AdminFeedbackDto
{
    public int FeedbackId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsResolved { get; set; }
    public DateTime CreatedAt { get; set; }
}