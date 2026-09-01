

namespace BlogGenerator.ServiceModels.v1;

public class AdminStatisticsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int BlockedUsers { get; set; }
    public int TotalBlogs { get; set; }
    public int PublishedBlogs { get; set; }
    public int TotalViews { get; set; }
    public int TotalLikes { get; set; }
    public int TotalComments { get; set; }
    public int TotalReposts { get; set; }
    public int TotalPayments { get; set; }
    public decimal TotalRevenue { get; set; }
}