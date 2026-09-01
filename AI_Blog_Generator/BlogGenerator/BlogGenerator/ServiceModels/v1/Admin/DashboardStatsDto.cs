


namespace BlogGenerator.ServiceModels.v1;

public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalBlogs { get; set; }
    public int TotalPublishedBlogs { get; set; }
    public int TotalPayments { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalFeedback { get; set; }
    public int PendingFeedback { get; set; }
    public int TotalIssues { get; set; }
    public int PendingIssues { get; set; }
    public int TotalReports { get; set; }
    public int PendingReports { get; set; }
}