using BlogGenerator.DomainModels.v1;
using BlogGenerator.Enums;
using BlogGenerator.Interfaces;
using BlogGenerator.ServiceModels.v1;
using Microsoft.EntityFrameworkCore;
using BlogGenerator.DAL;
using CategoryEntity = BlogGenerator.DomainModels.v1.Category;
using BlogGenerator.ServiceModels.v1.Category;

namespace BlogGenerator.BAL;

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;

    public AdminService(ApplicationDbContext context)
    {
        _context = context;
    }

    // ============================================================
    // DASHBOARD
    // ============================================================

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        return new DashboardStatsDto
        {
            TotalUsers = await _context.Users
                .CountAsync(),

            TotalBlogs = await _context.Blogs
                .CountAsync(),

            TotalPublishedBlogs = await _context.Blogs
                .CountAsync(x => x.Status == BlogStatus.Published),

            TotalPayments = await _context.Payments
                .CountAsync(),

            TotalRevenue = await _context.Payments
                .Where(x => x.PaymentStatus == PaymentStatus.Completed)
                .SumAsync(x => (decimal?)x.Amount) ?? 0,

            TotalFeedback = await _context.Feedbacks
                .CountAsync(),

            PendingFeedback = await _context.Feedbacks
                .CountAsync(x => x.Status == FeedbackStatus.Pending),

            TotalIssues = await _context.Issues
                .CountAsync(),

            PendingIssues = await _context.Issues
                .CountAsync(x =>
                    x.Status != IssueStatus.Resolved &&
                    x.Status != IssueStatus.Closed),

            TotalReports = await _context.BlogReports
                .CountAsync(),

            PendingReports = await _context.BlogReports
                .CountAsync(x => x.ReportStatus == ReportStatus.Pending)
        };
    }


    // ============================================================
    // USERS
    // ============================================================

    public async Task<IEnumerable<AdminUserDto>> GetAllUsersAsync()
    {
        return await _context.Users
            .Where(x => !x.IsDeleted)
            .Select(x => new AdminUserDto
            {
                UserId = x.UserId,
                UserName = x.UserName,
                Email = x.Email,
                Role = x.Role.ToString(),
                ProfilePictureUrl = x.ProfilePictureUrl,
                AvailableCredits = x.AvailableCredits,

                // User model uses IsActive.
                // Blocked means the user is not active.
                IsBlocked = !x.IsActive,

                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }


    public async Task<AdminUserDetailsDto?> GetUserDetailsAsync(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                !x.IsDeleted);

        if (user == null)
            return null;

        return new AdminUserDetailsDto
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Email = user.Email,
            Role = user.Role.ToString(),
            ProfilePictureUrl = user.ProfilePictureUrl,
            AvailableCredits = user.AvailableCredits,

            // User model uses IsActive.
            IsBlocked = !user.IsActive,

            CreatedAt = user.CreatedAt,

            TotalBlogs = await _context.Blogs
                .CountAsync(x => x.UserId == userId),

            TotalLikes = await _context.Likes
                .CountAsync(x => x.UserId == userId),

            TotalComments = await _context.Comments
                .CountAsync(x => x.UserId == userId),

            TotalPayments = await _context.Payments
                .CountAsync(x => x.UserId == userId)
        };
    }


    public async Task<IEnumerable<AdminPaymentDto>> GetUserPaymentsAsync(
        int userId)
    {
        return await _context.Payments
            .Include(x => x.User)
            .Where(x => x.UserId == userId)
            .Select(x => new AdminPaymentDto
            {
                PaymentId = x.PaymentId,
                UserId = x.UserId,
                UserName = x.User.UserName,

                Amount = x.Amount,
                Status = x.PaymentStatus.ToString(),

                RazorpayOrderId = x.RazorpayOrderId,

                // Actual Payment property is RazorpayPaymentId
                RazorpayPaymentId = x.RazorpayPaymentId ?? string.Empty,

                // DTO uses CreatedAt.
                // Payment entity uses PurchasedAt.
                CreatedAt = x.PurchasedAt
            })
            .ToListAsync();
    }


    public async Task<bool> BlockUserAsync(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                !x.IsDeleted);

        if (user == null)
            return false;

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }


    public async Task<bool> UnblockUserAsync(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                !x.IsDeleted);

        if (user == null)
            return false;

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }


    // ============================================================
    // BLOGS
    // ============================================================

    public async Task<IEnumerable<AdminBlogDto>> GetAllBlogsAsync()
    {
        return await _context.Blogs
            .Include(x => x.User)
            .Select(x => new AdminBlogDto
            {
                BlogId = x.BlogId,
                UserId = x.UserId,
                UserName = x.User.UserName,

                Title = x.Title,
                Excerpt = x.Excerpt,

                // AdminBlogDto uses IsPublished.
                // Blog entity uses Status.
                IsPublished = x.Status == BlogStatus.Published,

                CreatedAt = x.CreatedAt,
                PublishedAt = x.PublishedAt
            })
            .ToListAsync();
    }


    public async Task<AdminBlogDetailsDto?> GetBlogDetailsAsync(
        int blogId)
    {
        return await _context.Blogs
            .Include(x => x.User)
            .Where(x => x.BlogId == blogId)
            .Select(x => new AdminBlogDetailsDto
            {
                BlogId = x.BlogId,
                UserId = x.UserId,
                UserName = x.User.UserName,

                Title = x.Title,
                Excerpt = x.Excerpt,
                Content = x.Content,

                // Blog entity uses Status.
                IsPublished = x.Status == BlogStatus.Published,

                // DTO property -> Blog entity property
                Views = x.ViewsCount,
                Likes = x.LikesCount,
                Comments = x.CommentsCount,
                Reposts = x.RepostsCount,

                CreatedAt = x.CreatedAt,
                PublishedAt = x.PublishedAt
            })
            .FirstOrDefaultAsync();
    }


    public async Task<bool> DeleteBlogAsync(int blogId)
    {
        var blog = await _context.Blogs
            .FirstOrDefaultAsync(x => x.BlogId == blogId);

        if (blog == null)
            return false;

        // Delete reports associated with this blog
        var reports = await _context.BlogReports
            .Where(x => x.BlogId == blogId)
            .ToListAsync();

        if (reports.Any())
        {
            _context.BlogReports.RemoveRange(reports);
        }

        // Delete the blog
        _context.Blogs.Remove(blog);

        await _context.SaveChangesAsync();

        return true;
    }


    // ============================================================
    // PAYMENTS
    // ============================================================

    public async Task<IEnumerable<AdminPaymentDto>> GetAllPaymentsAsync()
    {
        return await _context.Payments
            .Include(x => x.User)
            .Include(x => x.Plan)
            .Select(x => new AdminPaymentDto
            {
                PaymentId = x.PaymentId,
                UserId = x.UserId,
                UserName = x.User.UserName,

                Amount = x.Amount,
                Status = x.PaymentStatus.ToString(),

                RazorpayOrderId = x.RazorpayOrderId,

                // Actual property name in Payment entity
                RazorpayPaymentId = x.RazorpayPaymentId ?? string.Empty,

                // AdminPaymentDto uses CreatedAt
                CreatedAt = x.PurchasedAt
            })
            .ToListAsync();
    }


    // ============================================================
    // DELETED USERS
    // ============================================================

    public async Task<IEnumerable<DeletedUserDto>> GetDeletedUsersAsync()
    {
        return await _context.DeletedAccounts
            .Select(x => new DeletedUserDto
            {
                // DeletedAccount entity uses DeletedId
                // DTO uses DeletedAccountId
                DeletedAccountId = x.DeletedId,

                UserId = x.UserId,
                Email = x.Email,
                Reason = x.Reason,
                DeletedAt = x.DeletedAt
            })
            .ToListAsync();
    }


    // ============================================================
    // FEEDBACK
    // ============================================================

    public async Task<IEnumerable<AdminFeedbackDto>> GetAllFeedbackAsync()
    {
        return await _context.Feedbacks
            .Include(x => x.User)
            .Select(x => new AdminFeedbackDto
            {
                FeedbackId = x.FeedbackId,
                UserId = x.UserId,
                UserName = x.User.UserName,

                Message = x.Message,

                // AdminFeedbackDto uses IsResolved.
                // Feedback entity uses Status.
                IsResolved = x.Status == FeedbackStatus.Resolved,

                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }


    public async Task<bool> ResolveFeedbackAsync(int feedbackId)
    {
        var feedback = await _context.Feedbacks
            .FirstOrDefaultAsync(x => x.FeedbackId == feedbackId);

        if (feedback == null)
            return false;

        feedback.Status = FeedbackStatus.Resolved;
        feedback.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }


    // ============================================================
    // ISSUES
    // ============================================================

    public async Task<IEnumerable<AdminIssueDto>> GetAllIssuesAsync()
    {
        return await _context.Issues
            .Include(x => x.User)
            .Select(x => new AdminIssueDto
            {
                IssueId = x.IssueId,
                UserId = x.UserId,
                UserName = x.User.UserName,

                Subject = x.Subject,
                Description = x.Description,

                Status = x.Status.ToString(),

                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }


    public async Task<bool> ResolveIssueAsync(int issueId)
    {
        var issue = await _context.Issues
            .FirstOrDefaultAsync(x => x.IssueId == issueId);

        if (issue == null)
            return false;

        issue.Status = IssueStatus.Resolved;
        issue.ResolvedAt = DateTime.UtcNow;
        issue.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }


    // ============================================================
    // PLANS
    // ============================================================

    public async Task<PlanResponseDto> CreatePlanAsync(
        CreatePlanRequestDto dto)
    {
        var plan = new Plan
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Credits = dto.Credits,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Plans.Add(plan);

        await _context.SaveChangesAsync();

        return new PlanResponseDto
        {
            PlanId = plan.PlanId,
            Name = plan.Name,
            Description = plan.Description,
            Price = plan.Price,
            Credits = plan.Credits,
            IsActive = plan.IsActive,
            CreatedAt = plan.CreatedAt
        };
    }


    public async Task<PlanResponseDto?> UpdatePlanAsync(
        int planId,
        UpdatePlanRequestDto dto)
    {
        var plan = await _context.Plans
            .FirstOrDefaultAsync(x => x.PlanId == planId);

        if (plan == null)
            return null;

        plan.Name = dto.Name;
        plan.Description = dto.Description;
        plan.Price = dto.Price;
        plan.Credits = dto.Credits;

        await _context.SaveChangesAsync();

        return new PlanResponseDto
        {
            PlanId = plan.PlanId,
            Name = plan.Name,
            Description = plan.Description,
            Price = plan.Price,
            Credits = plan.Credits,
            IsActive = plan.IsActive,
            CreatedAt = plan.CreatedAt
        };
    }


    public async Task<bool> DeletePlanAsync(int planId)
    {
        var plan = await _context.Plans
            .FirstOrDefaultAsync(x => x.PlanId == planId);

        if (plan == null)
            return false;

        // Deactivate instead of deleting because
        // Payment has a foreign key to Plan.
        plan.IsActive = false;

        await _context.SaveChangesAsync();

        return true;
    }


    // ============================================================
    // REPORTED BLOGS
    // ============================================================

    public async Task<IEnumerable<ReportedBlogDto>> GetReportedBlogsAsync()
    {
        return await _context.BlogReports
            .Include(x => x.Blog)
            .Include(x => x.ReportedByUser)
            .Where(x => x.ReportStatus == ReportStatus.Pending)
            .Select(x => new ReportedBlogDto
            {
                ReportId = x.ReportId,

                BlogId = x.BlogId,
                BlogTitle = x.Blog.Title,

                ReportedByUserId = x.ReportedByUserId,
                ReportedByUserName = x.ReportedByUser.UserName,

                Reason = x.Reason.ToString(),

                Status = x.ReportStatus.ToString(),

                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }


    public async Task<bool> ResolveReportedBlogAsync(int reportId)
    {
        var report = await _context.BlogReports
            .FirstOrDefaultAsync(x => x.ReportId == reportId);

        if (report == null)
            return false;

        report.ReportStatus = ReportStatus.Reviewed;

        await _context.SaveChangesAsync();

        return true;
    }


    // ============================================================
    // STATISTICS
    // ============================================================

    public async Task<AdminStatisticsDto> GetStatisticsAsync()
    {
        return new AdminStatisticsDto
        {
            TotalUsers = await _context.Users
                .CountAsync(x => !x.IsDeleted),

            ActiveUsers = await _context.Users
                .CountAsync(x =>
                    !x.IsDeleted &&
                    x.IsActive),

            BlockedUsers = await _context.Users
                .CountAsync(x =>
                    !x.IsDeleted &&
                    !x.IsActive),

            TotalBlogs = await _context.Blogs
                .CountAsync(),

            PublishedBlogs = await _context.Blogs
                .CountAsync(x => x.Status == BlogStatus.Published),

            TotalViews = await _context.Blogs
                .SumAsync(x => x.ViewsCount),

            TotalLikes = await _context.Blogs
                .SumAsync(x => x.LikesCount),

            TotalComments = await _context.Blogs
                .SumAsync(x => x.CommentsCount),

            TotalReposts = await _context.Blogs
                .SumAsync(x => x.RepostsCount),

            TotalPayments = await _context.Payments
                .CountAsync(),

            TotalRevenue = await _context.Payments
                .Where(x => x.PaymentStatus == PaymentStatus.Completed)
                .SumAsync(x => (decimal?)x.Amount) ?? 0
        };
    }

    public async Task<CategoryResponseDto> AddCategoryAsync(
        CategoryRequestDto request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Category name is required.");

        var categoryName = request.Name.Trim();

        var categoryExists = await _context.Categories
            .AnyAsync(x => x.Name == categoryName);

        if (categoryExists)
            throw new InvalidOperationException(
                "Category already exists.");

        var category = new CategoryEntity
        {
            Name = categoryName,
            Description = request.Description?.Trim(),
            Icon = request.Icon?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Categories.Add(category);

        await _context.SaveChangesAsync();

        return new CategoryResponseDto
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            Description = category.Description,
            Icon = category.Icon,
            CreatedAt = category.CreatedAt
        };
    }
}
