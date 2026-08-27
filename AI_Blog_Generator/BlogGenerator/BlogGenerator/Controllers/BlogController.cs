using BlogGenerator.Interfaces.Blog;
using BlogGenerator.ServiceModels.v1.Blog;
using BlogGenerator.ServiceModels.v1.Foundation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogGenerator.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[Controller]")]
public class BlogsController : ControllerBase
{
    private readonly IBlogService _blogService;

    public BlogsController(IBlogService blogService)
    {
        _blogService = blogService;
    }
    private int UserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    [HttpGet]
    public async Task<IActionResult> GetBlogs()
    {
        var result = await _blogService.GetBlogsAsync(UserId);

        return Ok(new ApiResponse<List<BlogListDto>>
        {
            Success = true,
            Message = "Blogs retrieved successfully.",
            Data = result
        });
    }

    [HttpGet("{blogId}")]
    public async Task<IActionResult> GetBlog(int blogId)
    {
        var result = await _blogService.GetBlogByIdAsync(UserId, blogId);

        return Ok(new ApiResponse<BlogDetailsDto>
        {
            Success = true,
            Message = "Blog retrieved successfully.",
            Data = result
        });
    }

    [HttpGet("{blogId}/versions")]
    public async Task<IActionResult> GetVersions(int blogId)
    {
        var result = await _blogService.GetBlogVersionsAsync(UserId, blogId);

        return Ok(new ApiResponse<List<BlogVersionDto>>
        {
            Success = true,
            Message = "Blog versions retrieved successfully.",
            Data = result
        });
    }

    [HttpGet("{blogId}/images")]
    public async Task<IActionResult> GetImages(int blogId)
    {
        var result = await _blogService.GetBlogImagesAsync(UserId, blogId);

        return Ok(new ApiResponse<List<BlogImageDto>>
        {
            Success = true,
            Message = "Blog images retrieved successfully.",
            Data = result
        });
    }

    [HttpGet("{blogId}/download-pdf")]
    public async Task<IActionResult> DownloadPdf(int blogId)
    {
        var pdf = await _blogService.DownloadPdfAsync(UserId, blogId);

        return File(
            pdf,
            "application/pdf",
            $"Blog_{blogId}.pdf");
    }

    [HttpDelete("{blogId}")]
    public async Task<IActionResult> DeleteBlog(int blogId)
    {
        await _blogService.DeleteBlogAsync(UserId, blogId);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Blog deleted successfully."
        });
    }

    [HttpDelete("images/{imageId}")]
    public async Task<IActionResult> DeleteImage(int imageId)
    {
        await _blogService.DeleteImageAsync(UserId, imageId);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Image deleted successfully."
        });
    }

    [HttpPost("{blogId}/publish")]
    public async Task<IActionResult> PublishBlog(int blogId)
    {
        var userId = UserId;

        var result = await _blogService
            .PublishBlogAsync(blogId, userId);

        return Ok(new ApiResponse<BlogResponseDto>
        {
            Success = true,
            Message = "Blog published successfully.",
            Data = result
        });
    }

    [HttpPost("{blogId}/unpublish")]
    public async Task<IActionResult> UnpublishBlog(int blogId)
    {
        var userId = UserId;

        var result = await _blogService
            .UnpublishBlogAsync(blogId, userId);

        return Ok(new ApiResponse<BlogResponseDto>
        {
            Success = true,
            Message = "Blog unpublished successfully.",
            Data = result
        });
    }

    [HttpPut("{blogId}")]
    public async Task<IActionResult> UpdateBlog(
    int blogId,
    [FromBody] UpdateBlogRequestDto request)
    {
        var userId = UserId;

        var result = await _blogService
            .UpdateBlogAsync(
                blogId,
                userId,
                request);

        return Ok(new ApiResponse<BlogResponseDto>
        {
            Success = true,
            Message = "Blog updated successfully.",
            Data = result
        });
    }

    [HttpGet("drafts")]
    public async Task<IActionResult> GetDraftBlogs()
    {
        var userId = UserId;

        var result = await _blogService
            .GetDraftBlogsAsync(userId);

        return Ok(new ApiResponse<List<BlogResponseDto>>
        {
            Success = true,
            Message = "Draft blogs retrieved successfully.",
            Data = result
        });
    }

    [HttpGet("published")]
    public async Task<IActionResult> GetPublishedBlogs()
    {
        var userId = UserId;

        var result = await _blogService
            .GetPublishedBlogsAsync(userId);

        return Ok(new ApiResponse<List<BlogResponseDto>>
        {
            Success = true,
            Message = "Published blogs retrieved successfully.",
            Data = result
        });
    }
}