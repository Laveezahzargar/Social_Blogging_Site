using BlogGenerator.DAL;
using BlogGenerator.Enums;
using BlogGenerator.Interfaces;
using BlogGenerator.ServiceModels.v1.PublicFeed;
using Microsoft.EntityFrameworkCore;

namespace BlogGenerator.BAL;

public class PublicFeedService : IPublicFeedService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PublicFeedService> _logger;

    public PublicFeedService(
        ApplicationDbContext context,
        ILogger<PublicFeedService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // =========================================================
    // 1. GET PUBLIC FEED
    // GET /api/feed
    // =========================================================

    public async Task<List<FeedBlogDto>> GetFeedAsync()
    {
        return await _context.Blogs
            .AsNoTracking()
            .Where(x =>
                x.Status == BlogStatus.Published &&
                x.Visibility == BlogVisibility.Public)
            .OrderByDescending(x => x.PublishedAt)
            .Select(x => new FeedBlogDto
            {
                BlogId = x.BlogId,
                Title = x.Title,
                Slug = x.Slug,
                Excerpt = x.Excerpt,

                CoverImageUrl = x.BlogImages
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault(),

                UserId = x.UserId,
                AuthorName = x.User.UserName,

                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name,

                WordCount = x.WordCount,
                PublishedAt = x.PublishedAt,

                ViewCount = x.ViewsCount,
                LikeCount = x.LikesCount,
                CommentCount = x.CommentsCount,

                Tags = new List<string>()
            })
            .ToListAsync();
    }

    // =========================================================
    // 2. GET TRENDING
    // GET /api/feed/trending
    // =========================================================

    public async Task<List<FeedBlogDto>> GetTrendingAsync()
    {
        return await _context.Blogs
            .AsNoTracking()
            .Where(x =>
                x.Status == BlogStatus.Published &&
                x.Visibility == BlogVisibility.Public)
            .OrderByDescending(x =>
                x.ViewsCount +
                (x.LikesCount * 3) +
                (x.CommentsCount * 5))
            .Select(x => new FeedBlogDto
            {
                BlogId = x.BlogId,
                Title = x.Title,
                Slug = x.Slug,
                Excerpt = x.Excerpt,

                CoverImageUrl = x.BlogImages
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault(),

                UserId = x.UserId,
                AuthorName = x.User.UserName,

                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name,

                WordCount = x.WordCount,
                PublishedAt = x.PublishedAt,

                ViewCount = x.ViewsCount,
                LikeCount = x.LikesCount,
                CommentCount = x.CommentsCount,

                Tags = new List<string>()
            })
            .ToListAsync();
    }

    // =========================================================
    // 3. FOLLOWING FEED
    // GET /api/feed/following
    // =========================================================

    public async Task<List<FeedBlogDto>> GetFollowingFeedAsync(
        int userId)
    {
        var followingUserIds = await _context.Follows
            .Where(x => x.FollowerUserId == userId)
            .Select(x => x.FollowingUserId)
            .ToListAsync();

        return await _context.Blogs
            .AsNoTracking()
            .Where(x =>
                followingUserIds.Contains(x.UserId) &&
                x.Status == BlogStatus.Published &&
                x.Visibility == BlogVisibility.Public)
            .OrderByDescending(x => x.PublishedAt)
            .Select(x => new FeedBlogDto
            {
                BlogId = x.BlogId,
                Title = x.Title,
                Slug = x.Slug,
                Excerpt = x.Excerpt,

                CoverImageUrl = x.BlogImages
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault(),

                UserId = x.UserId,
                AuthorName = x.User.UserName,

                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name,

                WordCount = x.WordCount,
                PublishedAt = x.PublishedAt,

                ViewCount = x.ViewsCount,
                LikeCount = x.LikesCount,
                CommentCount = x.CommentsCount,

                Tags = new List<string>()
            })
            .ToListAsync();
    }

    // =========================================================
    // 4. GET BY CATEGORY
    // GET /api/feed/category/{category}
    // =========================================================

    public async Task<List<FeedBlogDto>> GetByCategoryAsync(
        string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException(
                "Category is required.");

        category = category.Trim();

        return await _context.Blogs
            .AsNoTracking()
            .Where(x =>
                x.Status == BlogStatus.Published &&
                x.Visibility == BlogVisibility.Public &&
                x.Category.Name.ToLower() == category.ToLower())
            .OrderByDescending(x => x.PublishedAt)
            .Select(x => new FeedBlogDto
            {
                BlogId = x.BlogId,
                Title = x.Title,
                Slug = x.Slug,
                Excerpt = x.Excerpt,

                CoverImageUrl = x.BlogImages
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault(),

                UserId = x.UserId,
                AuthorName = x.User.UserName,

                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name,

                WordCount = x.WordCount,
                PublishedAt = x.PublishedAt,

                ViewCount = x.ViewsCount,
                LikeCount = x.LikesCount,
                CommentCount = x.CommentsCount,

                Tags = new List<string>()
            })
            .ToListAsync();
    }

    // =========================================================
    // 5. SEARCH
    // GET /api/feed/search
    // =========================================================

    public async Task<List<FeedBlogDto>> SearchAsync(
        string search)
    {
        if (string.IsNullOrWhiteSpace(search))
            throw new ArgumentException(
                "Search term is required.");

        search = search.Trim();

        return await _context.Blogs
            .AsNoTracking()
            .Where(x =>
                x.Status == BlogStatus.Published &&
                x.Visibility == BlogVisibility.Public &&
                (
                    x.Title.Contains(search) ||
                    x.Content.Contains(search) ||
                    x.User.UserName.Contains(search) ||
                    x.Category.Name.Contains(search)
                ))
            .OrderByDescending(x => x.PublishedAt)
            .Select(x => new FeedBlogDto
            {
                BlogId = x.BlogId,
                Title = x.Title,
                Slug = x.Slug,
                Excerpt = x.Excerpt,

                CoverImageUrl = x.BlogImages
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault(),

                UserId = x.UserId,
                AuthorName = x.User.UserName,

                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name,

                WordCount = x.WordCount,
                PublishedAt = x.PublishedAt,

                ViewCount = x.ViewsCount,
                LikeCount = x.LikesCount,
                CommentCount = x.CommentsCount,

                Tags = new List<string>()
            })
            .ToListAsync();
    }

    // =========================================================
    // 6. GET PUBLISHED BLOG
    // GET /api/feed/{blogId}
    // =========================================================

    public async Task<FeedBlogDetailsDto> GetBlogAsync(
        int blogId)
    {
        var blog = await _context.Blogs
            .AsNoTracking()
            .Where(x =>
                x.BlogId == blogId &&
                x.Status == BlogStatus.Published &&
                x.Visibility == BlogVisibility.Public)
            .Select(x => new FeedBlogDetailsDto
            {
                BlogId = x.BlogId,
                Title = x.Title,
                Slug = x.Slug,

                Content = x.Content,
                Excerpt = x.Excerpt,

                CoverImageUrl = x.BlogImages
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault(),

                UserId = x.UserId,
                AuthorName = x.User.UserName,

                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name,

                WordCount = x.WordCount,
                PublishedAt = x.PublishedAt,

                ViewCount = x.ViewsCount,
                LikeCount = x.LikesCount,
                CommentCount = x.CommentsCount,

                Tags = new List<string>()
            })
            .FirstOrDefaultAsync();

        if (blog == null)
            throw new KeyNotFoundException(
                "Published blog not found.");

        return blog;
    }
}