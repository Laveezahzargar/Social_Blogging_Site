using BlogGenerator.ServiceModels.v1;
using BlogGenerator.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlogGenerator.ServiceModels.v1.Category;

namespace BlogGenerator.Controllers;

[ApiController]
[Route("api/v1/[Controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    // GET: /api/admin/dashboard
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var result = await _adminService.GetDashboardStatsAsync();

        return Ok(result);
    }

    // GET: /api/admin/users
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _adminService.GetAllUsersAsync();

        return Ok(result);
    }

    // GET: /api/admin/users/{userId}
    [HttpGet("users/{userId:int}")]
    public async Task<IActionResult> GetUserDetails(int userId)
    {
        var result = await _adminService.GetUserDetailsAsync(userId);

        if (result == null)
            return NotFound(new { message = "User not found." });

        return Ok(result);
    }

    // GET: /api/admin/users/{userId}/payments
    [HttpGet("users/{userId:int}/payments")]
    public async Task<IActionResult> GetUserPayments(int userId)
    {
        var result = await _adminService.GetUserPaymentsAsync(userId);

        return Ok(result);
    }

    // PUT: /api/admin/users/{userId}/block
    [HttpPut("users/{userId:int}/block")]
    public async Task<IActionResult> BlockUser(int userId)
    {
        var result = await _adminService.BlockUserAsync(userId);

        if (!result)
            return NotFound(new { message = "User not found." });

        return Ok(new { message = "User blocked successfully." });
    }

    // PUT: /api/admin/users/{userId}/unblock
    [HttpPut("users/{userId:int}/unblock")]
    public async Task<IActionResult> UnblockUser(int userId)
    {
        var result = await _adminService.UnblockUserAsync(userId);

        if (!result)
            return NotFound(new { message = "User not found." });

        return Ok(new { message = "User unblocked successfully." });
    }

    // GET: /api/admin/blogs
    [HttpGet("blogs")]
    public async Task<IActionResult> GetAllBlogs()
    {
        var result = await _adminService.GetAllBlogsAsync();

        return Ok(result);
    }

    // GET: /api/admin/blogs/{blogId}
    [HttpGet("blogs/{blogId:int}")]
    public async Task<IActionResult> GetBlogDetails(int blogId)
    {
        var result = await _adminService.GetBlogDetailsAsync(blogId);

        if (result == null)
            return NotFound(new { message = "Blog not found." });

        return Ok(result);
    }

    // DELETE: /api/admin/blogs/{blogId}
    [HttpDelete("blogs/{blogId:int}")]
    public async Task<IActionResult> DeleteBlog(int blogId)
    {
        var result = await _adminService.DeleteBlogAsync(blogId);

        if (!result)
            return NotFound(new { message = "Blog not found." });

        return Ok(new { message = "Blog removed successfully." });
    }

    // GET: /api/admin/payments
    [HttpGet("payments")]
    public async Task<IActionResult> GetAllPayments()
    {
        var result = await _adminService.GetAllPaymentsAsync();

        return Ok(result);
    }

    // GET: /api/admin/deleted-users
    [HttpGet("deleted-users")]
    public async Task<IActionResult> GetDeletedUsers()
    {
        var result = await _adminService.GetDeletedUsersAsync();

        return Ok(result);
    }

    // GET: /api/admin/feedback
    [HttpGet("feedback")]
    public async Task<IActionResult> GetAllFeedback()
    {
        var result = await _adminService.GetAllFeedbackAsync();

        return Ok(result);
    }

    // PUT: /api/admin/feedback/{feedbackId}/resolve
    [HttpPut("feedback/{feedbackId:int}/resolve")]
    public async Task<IActionResult> ResolveFeedback(int feedbackId)
    {
        var result = await _adminService.ResolveFeedbackAsync(feedbackId);

        if (!result)
            return NotFound(new { message = "Feedback not found." });

        return Ok(new { message = "Feedback resolved successfully." });
    }

    // GET: /api/admin/issues
    [HttpGet("issues")]
    public async Task<IActionResult> GetAllIssues()
    {
        var result = await _adminService.GetAllIssuesAsync();

        return Ok(result);
    }

    // PUT: /api/admin/issues/{issueId}/resolve
    [HttpPut("issues/{issueId:int}/resolve")]
    public async Task<IActionResult> ResolveIssue(int issueId)
    {
        var result = await _adminService.ResolveIssueAsync(issueId);

        if (!result)
            return NotFound(new { message = "Issue not found." });

        return Ok(new { message = "Issue resolved successfully." });
    }

    // POST: /api/admin/plans
    [HttpPost("plans")]
    public async Task<IActionResult> CreatePlan(
    [FromBody] CreatePlanRequestDto dto)
    {
        var result = await _adminService.CreatePlanAsync(dto);

        return Ok(result);
    }

    [HttpPut("plans/{planId}")]
    public async Task<IActionResult> UpdatePlan(
        int planId,
        [FromBody] UpdatePlanRequestDto dto)
    {
        var result = await _adminService.UpdatePlanAsync(planId, dto);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    // DELETE: /api/admin/plans/{planId}
    [HttpDelete("plans/{planId:int}")]
    public async Task<IActionResult> DeletePlan(int planId)
    {
        var result = await _adminService.DeletePlanAsync(planId);

        if (!result)
            return NotFound(new { message = "Plan not found." });

        return Ok(new { message = "Plan deleted successfully." });
    }

    // GET: /api/admin/reported-blogs
    [HttpGet("reported-blogs")]
    public async Task<IActionResult> GetReportedBlogs()
    {
        var result = await _adminService.GetReportedBlogsAsync();

        return Ok(result);
    }

    // PUT: /api/admin/reported-blogs/{reportId}/resolve
    [HttpPut("reported-blogs/{reportId:int}/resolve")]
    public async Task<IActionResult> ResolveReportedBlog(int reportId)
    {
        var result = await _adminService.ResolveReportedBlogAsync(reportId);

        if (!result)
            return NotFound(new { message = "Report not found." });

        return Ok(new { message = "Reported blog resolved successfully." });
    }

    // GET: /api/admin/statistics
    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        var result = await _adminService.GetStatisticsAsync();

        return Ok(result);
    }

    [HttpPost("category")]
    public async Task<IActionResult> AddCategory(
        [FromBody] CategoryRequestDto request)
    {
        var result = await _adminService
            .AddCategoryAsync(request);

        return Ok(result);
    }
}