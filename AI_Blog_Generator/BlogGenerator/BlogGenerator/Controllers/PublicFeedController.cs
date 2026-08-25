using BlogGenerator.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogGenerator.Controllers;

[ApiController]
[Route("api/feed")]
public class PublicFeedController : ControllerBase
{
    private readonly IPublicFeedService _feedService;

    public PublicFeedController(IPublicFeedService feedService)
    {
        _feedService = feedService;
    }

    // =========================================================
    // GET PUBLIC FEED
    // GET /api/feed
    // =========================================================

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetFeed()
    {
        var result =
            await _feedService.GetFeedAsync();

        return Ok(result);
    }

    // =========================================================
    // GET TRENDING
    // GET /api/feed/trending
    // =========================================================

    [HttpGet("trending")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTrending()
    {
        var result =
            await _feedService.GetTrendingAsync();

        return Ok(result);
    }

    // =========================================================
    // GET FOLLOWING FEED
    // GET /api/feed/following
    // =========================================================

    [HttpGet("following")]
    [Authorize]
    public async Task<IActionResult> GetFollowing()
    {
        var userId = GetUserId();

        var result =
            await _feedService.GetFollowingFeedAsync(userId);

        return Ok(result);
    }

    // =========================================================
    // GET BY CATEGORY
    // GET /api/feed/category/{category}
    // =========================================================

    [HttpGet("category/{category}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByCategory(
        string category)
    {
        var result =
            await _feedService.GetByCategoryAsync(category);

        return Ok(result);
    }

    // =========================================================
    // SEARCH
    // GET /api/feed/search?search=technology
    // =========================================================

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search(
        [FromQuery] string search)
    {
        var result =
            await _feedService.SearchAsync(search);

        return Ok(result);
    }

    // =========================================================
    // GET PUBLISHED BLOG
    // GET /api/feed/{blogId}
    // =========================================================

    [HttpGet("{blogId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBlog(
        int blogId)
    {
        var result =
            await _feedService.GetBlogAsync(blogId);

        return Ok(result);
    }

    // =========================================================
    // HELPER
    // =========================================================

    private int GetUserId()
    {
        var userIdClaim =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException(
                "User ID not found in token.");

        return userId;
    }
}