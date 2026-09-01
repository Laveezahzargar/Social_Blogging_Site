

namespace BlogGenerator.ServiceModels.v1;

public class ReportedBlogDto
{
    public int ReportId { get; set; }
    public int BlogId { get; set; }
    public string BlogTitle { get; set; } = string.Empty;
    public int ReportedByUserId { get; set; }
    public string ReportedByUserName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}