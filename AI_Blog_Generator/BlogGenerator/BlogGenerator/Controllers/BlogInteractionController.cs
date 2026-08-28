using BlogGenerator.Interfaces;
using BlogGenerator.ServiceModels.v1.BlogInteraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogGenerator.Controllers
{
    [ApiController]
    [Route("api/v1/[Controller]")]
    public class BlogInteractionController : ControllerBase
    {
        private readonly IBlogInteractionService _service;

        public BlogInteractionController(
            IBlogInteractionService service)
        {
            _service = service;
        }

        private int CurrentUserId =>
            int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );


        // =========================================================
        // LIKE BLOG
        // POST /api/blogs/{blogId}/like
        // =========================================================

        [Authorize]
        [HttpPost("blogs/{blogId}/like")]
        public async Task<IActionResult> LikeBlog(
            int blogId)
        {
            await _service.LikeBlogAsync(
                blogId,
                CurrentUserId);

            return Ok(new
            {
                message = "Blog liked successfully."
            });
        }


        // =========================================================
        // UNLIKE BLOG
        // DELETE /api/blogs/{blogId}/like
        // =========================================================

        [Authorize]
        [HttpDelete("blogs/{blogId}/like")]
        public async Task<IActionResult> UnlikeBlog(
            int blogId)
        {
            await _service.UnlikeBlogAsync(
                blogId,
                CurrentUserId);

            return Ok(new
            {
                message = "Blog unliked successfully."
            });
        }


        // =========================================================
        // GET BLOG LIKES
        // GET /api/blogs/{blogId}/likes
        // =========================================================

        [AllowAnonymous]
        [HttpGet("blogs/{blogId}/likes")]
        public async Task<IActionResult> GetBlogLikes(
            int blogId)
        {
            var result =
                await _service.GetBlogLikesAsync(blogId);

            return Ok(result);
        }


        // =========================================================
        // ADD COMMENT
        // POST /api/blogs/{blogId}/comments
        // =========================================================

        [Authorize]
        [HttpPost("blogs/{blogId}/comments")]
        public async Task<IActionResult> AddComment(
            int blogId,
            [FromBody] CreateCommentDto dto)
        {
            var result =
                await _service.AddCommentAsync(
                    blogId,
                    CurrentUserId,
                    dto);

            return Ok(result);
        }


        // =========================================================
        // GET COMMENTS
        // GET /api/blogs/{blogId}/comments
        // =========================================================

        [AllowAnonymous]
        [HttpGet("blogs/{blogId}/comments")]
        public async Task<IActionResult> GetComments(
            int blogId)
        {
            int? userId = null;

            if (User.Identity?.IsAuthenticated == true)
            {
                userId = CurrentUserId;
            }

            var result =
                await _service.GetCommentsAsync(
                    blogId,
                    userId);

            return Ok(result);
        }


        // =========================================================
        // UPDATE COMMENT
        // PUT /api/comments/{commentId}
        // =========================================================

        [Authorize]
        [HttpPut("comments/{commentId}")]
        public async Task<IActionResult> UpdateComment(
            int commentId,
            [FromBody] UpdateCommentDto dto)
        {
            var result =
                await _service.UpdateCommentAsync(
                    commentId,
                    CurrentUserId,
                    dto);

            return Ok(result);
        }


        // =========================================================
        // DELETE COMMENT
        // DELETE /api/comments/{commentId}
        // =========================================================

        [Authorize]
        [HttpDelete("comments/{commentId}")]
        public async Task<IActionResult> DeleteComment(
            int commentId)
        {
            await _service.DeleteCommentAsync(
                commentId,
                CurrentUserId);

            return Ok(new
            {
                message = "Comment deleted successfully."
            });
        }


        // =========================================================
        // SAVE BLOG
        // POST /api/blogs/{blogId}/save
        // =========================================================

        [Authorize]
        [HttpPost("blogs/{blogId}/save")]
        public async Task<IActionResult> SaveBlog(
            int blogId)
        {
            await _service.SaveBlogAsync(
                blogId,
                CurrentUserId);

            return Ok(new
            {
                message = "Blog saved successfully."
            });
        }


        // =========================================================
        // UNSAVE BLOG
        // DELETE /api/blogs/{blogId}/save
        // =========================================================

        [Authorize]
        [HttpDelete("blogs/{blogId}/save")]
        public async Task<IActionResult> UnsaveBlog(
            int blogId)
        {
            await _service.UnsaveBlogAsync(
                blogId,
                CurrentUserId);

            return Ok(new
            {
                message = "Blog removed from saved blogs."
            });
        }


        // =========================================================
        // REPOST BLOG
        // POST /api/blogs/{blogId}/repost
        // =========================================================

        [Authorize]
        [HttpPost("blogs/{blogId}/repost")]
        public async Task<IActionResult> RepostBlog(
            int blogId)
        {
            await _service.RepostBlogAsync(
                blogId,
                CurrentUserId);

            return Ok(new
            {
                message = "Blog reposted successfully."
            });
        }


        // =========================================================
        // REMOVE REPOST
        // DELETE /api/blogs/{blogId}/repost
        // =========================================================

        [Authorize]
        [HttpDelete("blogs/{blogId}/repost")]
        public async Task<IActionResult> RemoveRepost(
            int blogId)
        {
            await _service.RemoveRepostAsync(
                blogId,
                CurrentUserId);

            return Ok(new
            {
                message = "Repost removed successfully."
            });
        }


        // =========================================================
        // REPORT BLOG
        // POST /api/blogs/{blogId}/report
        // =========================================================

        [Authorize]
        [HttpPost("blogs/{blogId}/report")]
        public async Task<IActionResult> ReportBlog(
            int blogId,
            [FromBody] ReportBlogDto dto)
        {
            await _service.ReportBlogAsync(
                blogId,
                CurrentUserId,
                dto);

            return Ok(new
            {
                message = "Blog reported successfully."
            });
        }


        // =========================================================
        // LIKE COMMENT
        // POST /api/comments/{commentId}/like
        // =========================================================

        [Authorize]
        [HttpPost("comments/{commentId}/like")]
        public async Task<IActionResult> LikeComment(
            int commentId)
        {
            await _service.LikeCommentAsync(
                commentId,
                CurrentUserId);

            return Ok(new
            {
                message = "Comment liked successfully."
            });
        }


        // =========================================================
        // UNLIKE COMMENT
        // DELETE /api/comments/{commentId}/like
        // =========================================================

        [Authorize]
        [HttpDelete("comments/{commentId}/like")]
        public async Task<IActionResult> UnlikeComment(
            int commentId)
        {
            await _service.UnlikeCommentAsync(
                commentId,
                CurrentUserId);

            return Ok(new
            {
                message = "Comment unliked successfully."
            });
        }
    }
}