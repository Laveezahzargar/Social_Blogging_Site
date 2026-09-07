using BlogGenerator.ServiceModels.v1;
using BlogGenerator.ServiceModels.v1.Category;

namespace BlogGenerator.Interfaces;

public interface IAdminService
{
    // Dashboard
    Task<DashboardStatsDto> GetDashboardStatsAsync();

    // Users
    Task<IEnumerable<AdminUserDto>> GetAllUsersAsync();
    Task<AdminUserDetailsDto?> GetUserDetailsAsync(int userId);
    Task<IEnumerable<AdminPaymentDto>> GetUserPaymentsAsync(int userId);
    Task<bool> BlockUserAsync(int userId);
    Task<bool> UnblockUserAsync(int userId);

    // Blogs
    Task<IEnumerable<AdminBlogDto>> GetAllBlogsAsync();
    Task<AdminBlogDetailsDto?> GetBlogDetailsAsync(int blogId);
    Task<bool> DeleteBlogAsync(int blogId);

    // Payments
    Task<IEnumerable<AdminPaymentDto>> GetAllPaymentsAsync();

    // Deleted users
    Task<IEnumerable<DeletedUserDto>> GetDeletedUsersAsync();

    // Feedback
    Task<IEnumerable<AdminFeedbackDto>> GetAllFeedbackAsync();
    Task<bool> ResolveFeedbackAsync(int feedbackId);

    // Issues
    Task<IEnumerable<AdminIssueDto>> GetAllIssuesAsync();
    Task<bool> ResolveIssueAsync(int issueId);

    // Plans
    Task<PlanResponseDto> CreatePlanAsync(CreatePlanRequestDto dto);

    Task<PlanResponseDto?> UpdatePlanAsync(
        int planId,
        UpdatePlanRequestDto dto);
    Task<bool> DeletePlanAsync(int planId);

    // Reported blogs
    Task<IEnumerable<ReportedBlogDto>> GetReportedBlogsAsync();
    Task<bool> ResolveReportedBlogAsync(int reportId);

    // Statistics
    Task<AdminStatisticsDto> GetStatisticsAsync();

    Task<CategoryResponseDto> AddCategoryAsync(
        CategoryRequestDto request);
}