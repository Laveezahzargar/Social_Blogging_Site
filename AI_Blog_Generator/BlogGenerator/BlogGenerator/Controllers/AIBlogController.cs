using BlogGenerator.Interfaces;
using BlogGenerator.ServiceModels.v1.AIBlog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogGenerator.Controllers;

[ApiController]
[Route("api/blogs")]
[Authorize]
public class AIBlogController : ControllerBase
{
    private readonly IAIBlogService _aiBlogService;

    public AIBlogController(IAIBlogService aiBlogService)
    {
        _aiBlogService = aiBlogService;
    }

    private int GetUserId()
    {
        return int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateBlog(
        [FromBody] GenerateBlogRequestDto request)
    {
        var userId = GetUserId();

        var result = await _aiBlogService
            .GenerateBlogAsync(userId, request);

        return Ok(result);
    }

    [HttpPost("{blogId}/regenerate")]
    public async Task<IActionResult> RegenerateBlog(int blogId)
    {
        var userId = GetUserId();

        var result = await _aiBlogService
            .RegenerateBlogAsync(userId, blogId);

        return Ok(result);
    }

    [HttpPost("{blogId}/expand")]
    public async Task<IActionResult> ExpandBlog(
        int blogId,
        [FromBody] BlogActionRequestDto? request)
    {
        var userId = GetUserId();

        var result = await _aiBlogService
            .ExpandBlogAsync(userId, blogId, request);

        return Ok(result);
    }

    [HttpPost("{blogId}/shorten")]
    public async Task<IActionResult> ShortenBlog(
        int blogId,
        [FromBody] BlogActionRequestDto? request)
    {
        var userId = GetUserId();

        var result = await _aiBlogService
            .ShortenBlogAsync(userId, blogId, request);

        return Ok(result);
    }

    [HttpPost("{blogId}/generate-image")]
    public async Task<IActionResult> GenerateImage(
        int blogId,
        [FromBody] GenerateImageRequestDto? request)
    {
        var userId = GetUserId();

        var result = await _aiBlogService
            .GenerateImageAsync(userId, blogId, request);

        return Ok(result);
    }

    [HttpGet("{blogId}/ai-history")]
    public async Task<IActionResult> GetAIHistory(int blogId)
    {
        var userId = GetUserId();

        var result = await _aiBlogService
            .GetAIHistoryAsync(userId, blogId);

        return Ok(result);
    }

    [HttpPost("{blogId}/rewrite")]
    public async Task<IActionResult> RewriteBlog(
        int blogId,
        [FromBody] RewriteBlogRequestDto request)
    {
        var userId = GetUserId();

        var result = await _aiBlogService
            .RewriteBlogAsync(userId, blogId, request);

        return Ok(result);
    }

    [HttpPost("{blogId}/generate-tags")]
    public async Task<IActionResult> GenerateTags(int blogId)
    {
        var userId = GetUserId();

        var result = await _aiBlogService
            .GenerateTagsAsync(userId, blogId);

        return Ok(result);
    }

    [HttpPost("{blogId}/generate-summary")]
    public async Task<IActionResult> GenerateSummary(int blogId)
    {
        var userId = GetUserId();

        var result = await _aiBlogService
            .GenerateSummaryAsync(userId, blogId);

        return Ok(result);
    }

    [HttpGet("{blogId}/tags")]
    public async Task<IActionResult> GetTags(int blogId)
    {
        var userId = GetUserId();

        var result = await _aiBlogService
            .GetTagsAsync(userId, blogId);

        return Ok(result);
    }

    // =========================================================
    // 12. PUBLISH BLOG
    // POST /api/blogs/{blogId}/publish
    // =========================================================

    [HttpPost("{blogId:int}/publish")]
    [Authorize]
    public async Task<IActionResult> PublishBlog(
        int blogId)
    {
        var userId = GetUserId();

        var result = await _aiBlogService.PublishBlogAsync(
            userId,
            blogId);

        return Ok(new
        {
            Success = result,
            Message = "Blog published successfully."
        });
    }
}