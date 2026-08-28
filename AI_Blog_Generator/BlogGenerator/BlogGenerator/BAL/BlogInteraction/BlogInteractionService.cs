using BlogGenerator.DAL;
using BlogGenerator.DomainModels.v1;
using BlogGenerator.Enums;
using BlogGenerator.Interfaces;
using BlogGenerator.ServiceModels.v1.BlogInteraction;
using Microsoft.EntityFrameworkCore;

namespace BlogGenerator.BAL.BlogInteraction
{
    public class BlogInteractionService : IBlogInteractionService
    {
        private readonly ApplicationDbContext _context;

        public BlogInteractionService(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // LIKE BLOG
        // POST /api/blogs/{blogId}/like
        // =========================================================

        public async Task LikeBlogAsync(int blogId, int userId)
        {
            var blog = await _context.Blogs
                .FirstOrDefaultAsync(b => b.BlogId == blogId);

            if (blog == null)
                throw new KeyNotFoundException("Blog not found.");

            //if (!blog.IsPublished)
            //    throw new InvalidOperationException(
            //        "Only published blogs can be liked.");

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(x =>
                    x.BlogId == blogId &&
                    x.UserId == userId);

            if (existingLike != null)
                throw new InvalidOperationException(
                    "You have already liked this blog.");

            _context.Likes.Add(new Likes
            {
                BlogId = blogId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // UNLIKE BLOG
        // DELETE /api/blogs/{blogId}/like
        // =========================================================

        public async Task UnlikeBlogAsync(int blogId, int userId)
        {
            var like = await _context.Likes
                .FirstOrDefaultAsync(x =>
                    x.BlogId == blogId &&
                    x.UserId == userId);

            if (like == null)
                throw new KeyNotFoundException("Like not found.");

            _context.Likes.Remove(like);

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // GET BLOG LIKES
        // GET /api/blogs/{blogId}/likes
        // =========================================================

        public async Task<List<BlogLikeUserDto>>
            GetBlogLikesAsync(int blogId)
        {
            var blogExists = await _context.Blogs
                .AnyAsync(b => b.BlogId == blogId);

            if (!blogExists)
                throw new KeyNotFoundException("Blog not found.");

            return await _context.Likes
                .Where(x => x.BlogId == blogId)
                .Include(x => x.User)
                .Select(x => new BlogLikeUserDto
                {
                    UserId = x.UserId,
                    UserName = x.User.UserName,
                    ProfilePictureUrl =
                        x.User.ProfilePictureUrl
                })
                .ToListAsync();
        }

        // =========================================================
        // ADD COMMENT
        // POST /api/blogs/{blogId}/comments
        // =========================================================

        public async Task<CommentDto> AddCommentAsync(
            int blogId,
            int userId,
            CreateCommentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Content))
                throw new ArgumentException(
                    "Comment cannot be empty.");

            var blog = await _context.Blogs
                .FirstOrDefaultAsync(b => b.BlogId == blogId);

            if (blog == null)
                throw new KeyNotFoundException("Blog not found.");

            //if (!blog.IsPublished)
            //    throw new InvalidOperationException(
            //        "Comments are allowed only on published blogs.");

            var comment = new Comments
            {
                BlogId = blogId,
                UserId = userId,
                Content = dto.Content.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);

            await _context.SaveChangesAsync();

            return await GetCommentDtoAsync(
                comment.CommentId,
                userId);
        }

        // =========================================================
        // GET COMMENTS
        // GET /api/blogs/{blogId}/comments
        // =========================================================

        public async Task<List<CommentDto>>
            GetCommentsAsync(
                int blogId,
                int? currentUserId = null)
        {
            var blogExists = await _context.Blogs
                .AnyAsync(b => b.BlogId == blogId);

            if (!blogExists)
                throw new KeyNotFoundException("Blog not found.");

            return await _context.Comments
                .Where(c => c.BlogId == blogId)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    CommentId = c.CommentId,
                    BlogId = c.BlogId,
                    UserId = c.UserId,
                    UserName = c.User.UserName,
                    ProfilePictureUrl =
                        c.User.ProfilePictureUrl,
                    Content = c.Content,
                    LikeCount = _context.CommentLikes
                        .Count(l =>
                            l.CommentId == c.CommentId),
                    IsLikedByCurrentUser =
                        currentUserId.HasValue &&
                        _context.CommentLikes.Any(l =>
                            l.CommentId == c.CommentId &&
                            l.UserId == currentUserId.Value),
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();
        }

        // =========================================================
        // UPDATE COMMENT
        // PUT /api/comments/{commentId}
        // =========================================================

        public async Task<CommentDto> UpdateCommentAsync(
            int commentId,
            int userId,
            UpdateCommentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Content))
                throw new ArgumentException(
                    "Comment cannot be empty.");

            var comment = await _context.Comments
                .FirstOrDefaultAsync(c =>
                    c.CommentId == commentId);

            if (comment == null)
                throw new KeyNotFoundException(
                    "Comment not found.");

            if (comment.UserId != userId)
                throw new UnauthorizedAccessException(
                    "You can only edit your own comments.");

            comment.Content = dto.Content.Trim();
            comment.IsEdited = true;
            comment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetCommentDtoAsync(
                commentId,
                userId);
        }

        // =========================================================
        // DELETE COMMENT
        // DELETE /api/comments/{commentId}
        // =========================================================

        public async Task DeleteCommentAsync(
            int commentId,
            int userId)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c =>
                    c.CommentId == commentId);

            if (comment == null)
                throw new KeyNotFoundException(
                    "Comment not found.");

            if (comment.UserId != userId)
                throw new UnauthorizedAccessException(
                    "You can only delete your own comments.");

            _context.Comments.Remove(comment);

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // SAVE BLOG
        // POST /api/blogs/{blogId}/save
        // =========================================================

        public async Task SaveBlogAsync(
            int blogId,
            int userId)
        {
            var blog = await _context.Blogs
                .FirstOrDefaultAsync(b => b.BlogId == blogId);

            if (blog == null)
                throw new KeyNotFoundException(
                    "Blog not found.");

            //if (!blog.IsPublished)
            //    throw new InvalidOperationException(
            //        "Only published blogs can be saved.");

            var existing = await _context.Bookmarks
                .FirstOrDefaultAsync(x =>
                    x.BlogId == blogId &&
                    x.UserId == userId);

            if (existing != null)
                throw new InvalidOperationException(
                    "Blog is already saved.");

            _context.Bookmarks.Add(new Bookmarks
            {
                BlogId = blogId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // UNSAVE BLOG
        // DELETE /api/blogs/{blogId}/save
        // =========================================================

        public async Task UnsaveBlogAsync(
            int blogId,
            int userId)
        {
            var bookmark = await _context.Bookmarks
                .FirstOrDefaultAsync(x =>
                    x.BlogId == blogId &&
                    x.UserId == userId);

            if (bookmark == null)
                throw new KeyNotFoundException(
                    "Saved blog not found.");

            _context.Bookmarks.Remove(bookmark);

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // REPOST BLOG
        // POST /api/blogs/{blogId}/repost
        // =========================================================

        public async Task RepostBlogAsync(
            int blogId,
            int userId)
        {
            var blog = await _context.Blogs
                .FirstOrDefaultAsync(b => b.BlogId == blogId);

            if (blog == null)
                throw new KeyNotFoundException(
                    "Blog not found.");

            //if (!blog.IsPublished)
            //    throw new InvalidOperationException(
            //        "Only published blogs can be reposted.");

            var existing = await _context.Reposts
                .FirstOrDefaultAsync(x =>
                    x.BlogId == blogId &&
                    x.UserId == userId);

            if (existing != null)
                throw new InvalidOperationException(
                    "Blog is already reposted.");

            _context.Reposts.Add(new Reposts
            {
                BlogId = blogId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // REMOVE REPOST
        // DELETE /api/blogs/{blogId}/repost
        // =========================================================

        public async Task RemoveRepostAsync(
            int blogId,
            int userId)
        {
            var repost = await _context.Reposts
                .FirstOrDefaultAsync(x =>
                    x.BlogId == blogId &&
                    x.UserId == userId);

            if (repost == null)
                throw new KeyNotFoundException(
                    "Repost not found.");

            _context.Reposts.Remove(repost);

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // REPORT BLOG
        // POST /api/blogs/{blogId}/report
        // =========================================================

        public async Task ReportBlogAsync(
            int blogId,
            int userId,
            ReportBlogDto dto)
        {
            var blogExists = await _context.Blogs
                .AnyAsync(b => b.BlogId == blogId);

            if (!blogExists)
                throw new KeyNotFoundException(
                    "Blog not found.");

            var alreadyReported =
                await _context.BlogReports.AnyAsync(x =>
                    x.BlogId == blogId &&
                    x.ReportedByUserId == userId);

            if (alreadyReported)
                throw new InvalidOperationException(
                    "You have already reported this blog.");

            if (!Enum.IsDefined(
                    typeof(ReportReason),
                    dto.Reason))
            {
                throw new ArgumentException(
                    "Invalid report reason.");
            }

            _context.BlogReports.Add(new BlogReports
            {
                BlogId = blogId,
                ReportedByUserId = userId,
                Reason = dto.Reason,
                Description = dto.Description,
                ReportStatus = ReportStatus.Pending,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // LIKE COMMENT
        // POST /api/comments/{commentId}/like
        // =========================================================

        public async Task LikeCommentAsync(
            int commentId,
            int userId)
        {
            var commentExists = await _context.Comments
                .AnyAsync(c =>
                    c.CommentId == commentId);

            if (!commentExists)
                throw new KeyNotFoundException(
                    "Comment not found.");

            var existing = await _context.CommentLikes
                .FirstOrDefaultAsync(x =>
                    x.CommentId == commentId &&
                    x.UserId == userId);

            if (existing != null)
                throw new InvalidOperationException(
                    "You have already liked this comment.");

            _context.CommentLikes.Add(new CommentLikes
            {
                CommentId = commentId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // UNLIKE COMMENT
        // DELETE /api/comments/{commentId}/like
        // =========================================================

        public async Task UnlikeCommentAsync(
            int commentId,
            int userId)
        {
            var like = await _context.CommentLikes
                .FirstOrDefaultAsync(x =>
                    x.CommentId == commentId &&
                    x.UserId == userId);

            if (like == null)
                throw new KeyNotFoundException(
                    "Comment like not found.");

            _context.CommentLikes.Remove(like);

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // COMMENT DTO MAPPER
        // =========================================================

        private async Task<CommentDto> GetCommentDtoAsync(
            int commentId,
            int currentUserId)
        {
            return await _context.Comments
                .Where(c => c.CommentId == commentId)
                .Include(c => c.User)
                .Select(c => new CommentDto
                {
                    CommentId = c.CommentId,
                    BlogId = c.BlogId,
                    UserId = c.UserId,
                    UserName = c.User.UserName,
                    ProfilePictureUrl =
                        c.User.ProfilePictureUrl,
                    Content = c.Content,
                    LikeCount = _context.CommentLikes
                        .Count(l =>
                            l.CommentId == c.CommentId),
                    IsLikedByCurrentUser =
                        _context.CommentLikes.Any(l =>
                            l.CommentId == c.CommentId &&
                            l.UserId == currentUserId),
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .FirstAsync();
        }
    }
}