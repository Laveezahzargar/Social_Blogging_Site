using BlogGenerator.DAL;
using BlogGenerator.Enums;
using BlogGenerator.Interfaces;
using BlogGenerator.ServiceModels.v1.AIBlog;
using BlogGenerator.DomainModels.v1;
using Microsoft.EntityFrameworkCore;
using BlogGenerator.Interfaces;

using BlogEntity = BlogGenerator.DomainModels.v1.Blog;

namespace BlogGenerator.BAL;

public class AIBlogService : IAIBlogService
{
    private readonly ApplicationDbContext _context;
    private readonly IAIProviderService _aiProvider;
    private readonly ILogger<AIBlogService> _logger;
    private readonly IImageStorageService _imageStorageService;

    public AIBlogService(
        ApplicationDbContext context,
        IAIProviderService aiProvider,
        IImageStorageService imageStorageService,
        ILogger<AIBlogService> logger
        )
    {
        _context = context;
        _aiProvider = aiProvider;
        _imageStorageService = imageStorageService;
        _logger = logger;
    }

    // =========================================================
    // 1. GENERATE BLOG
    // =========================================================

    public async Task<GenerateBlogResponseDto> GenerateBlogAsync(
        int userId,
        GenerateBlogRequestDto request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                !x.IsDeleted &&
                x.IsActive);

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        var categoryExists = await _context.Categories
            .AnyAsync(x => x.CategoryId == request.CategoryId);

        if (!categoryExists)
            throw new KeyNotFoundException("Category not found.");

        const int creditsRequired = 5;

        if (user.AvailableCredits < creditsRequired)
            throw new InvalidOperationException(
                "Insufficient credits.");

        var prompt = BuildGenerationPrompt(request);

        var content = await _aiProvider.GenerateBlogAsync(prompt);

        if (string.IsNullOrWhiteSpace(content))
            throw new InvalidOperationException(
                "AI provider returned empty content.");

        var title = request.Topic.Trim();

        var blog = new BlogEntity
{
    UserId = userId,
    CategoryId = request.CategoryId,

    Title = title,
    Slug = GenerateSlug(title),

    Prompt = request.Topic,
    Content = content,
    Excerpt = CreateExcerpt(content),

    Tone = request.Tone.ToString(),
    Audience = request.Audience.ToString(),

    WordCount = CountWords(content),
    CreditsUsed = creditsRequired,
    Language = request.Language,

    Status = BlogStatus.Draft,
    Visibility = BlogVisibility.Private,

    PublishedAt = null,

    CreatedAt = DateTime.UtcNow,
    UpdatedAt = DateTime.UtcNow
};

        user.AvailableCredits -= creditsRequired;

        _context.Blogs.Add(blog);

        await _context.SaveChangesAsync();

        var version = new BlogVersion
        {
            BlogId = blog.BlogId,
            Title = blog.Title,
            Content = blog.Content,
            WordCount = blog.WordCount,
            CreatedAt = DateTime.UtcNow
        };

        _context.BlogVersions.Add(version);

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Blog {BlogId} generated for user {UserId}. Credits used: {Credits}",
            blog.BlogId,
            userId,
            creditsRequired);

        return new GenerateBlogResponseDto
        {
            BlogId = blog.BlogId,
            Title = blog.Title,
            Content = blog.Content,
            Excerpt = blog.Excerpt,
            CreditsUsed = creditsRequired,
            RemainingCredits = user.AvailableCredits
        };
    }

    // =========================================================
    // 2. REGENERATE BLOG
    // =========================================================

    public async Task<GenerateBlogResponseDto> RegenerateBlogAsync(
        int userId,
        int blogId)
    {
        var blog = await GetUserBlogAsync(userId, blogId);

        const int creditsRequired = 5;

        var user = await GetUserAsync(userId);

        if (user.AvailableCredits < creditsRequired)
            throw new InvalidOperationException(
                "Insufficient credits.");

        var prompt =
            $"Regenerate the following blog with a fresh and improved version. " +
            $"Maintain the original topic and general purpose.\n\n" +
            $"Title: {blog.Title}\n\n" +
            $"Original Content:\n{blog.Content}";

        var content = await _aiProvider.GenerateBlogAsync(prompt);

        if (string.IsNullOrWhiteSpace(content))
            throw new InvalidOperationException(
                "AI provider returned empty content.");

        blog.Content = content;
        blog.Excerpt = CreateExcerpt(content);
        blog.WordCount = CountWords(content);
        blog.CreditsUsed += creditsRequired;
        blog.UpdatedAt = DateTime.UtcNow;

        user.AvailableCredits -= creditsRequired;

        var version = new BlogVersion
        {
            BlogId = blog.BlogId,
            Title = blog.Title,
            Content = content,
            WordCount = blog.WordCount,
            CreatedAt = DateTime.UtcNow
        };

        _context.BlogVersions.Add(version);

        await _context.SaveChangesAsync();

        return CreateResponse(
            blog,
            creditsRequired,
            user.AvailableCredits);
    }

    // =========================================================
    // 3. EXPAND BLOG
    // =========================================================

    public async Task<GenerateBlogResponseDto> ExpandBlogAsync(
        int userId,
        int blogId,
        BlogActionRequestDto? request)
    {
        var blog = await GetUserBlogAsync(userId, blogId);
        var user = await GetUserAsync(userId);

        const int creditsRequired = 2;

        if (user.AvailableCredits < creditsRequired)
            throw new InvalidOperationException(
                "Insufficient credits.");

        var content = await _aiProvider.ExpandBlogAsync(
            blog.Content,
            request?.Instructions);

        if (string.IsNullOrWhiteSpace(content))
            throw new InvalidOperationException(
                "AI provider returned empty content.");

        blog.Content = content;
        blog.Excerpt = CreateExcerpt(content);
        blog.WordCount = CountWords(content);
        blog.CreditsUsed += creditsRequired;
        blog.UpdatedAt = DateTime.UtcNow;

        user.AvailableCredits -= creditsRequired;

        _context.BlogVersions.Add(new BlogVersion
        {
            BlogId = blog.BlogId,
            Title = blog.Title,
            Content = content,
            WordCount = blog.WordCount,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return CreateResponse(
            blog,
            creditsRequired,
            user.AvailableCredits);
    }

    // =========================================================
    // 4. SHORTEN BLOG
    // =========================================================

    public async Task<GenerateBlogResponseDto> ShortenBlogAsync(
        int userId,
        int blogId,
        BlogActionRequestDto? request)
    {
        var blog = await GetUserBlogAsync(userId, blogId);
        var user = await GetUserAsync(userId);

        const int creditsRequired = 2;

        if (user.AvailableCredits < creditsRequired)
            throw new InvalidOperationException(
                "Insufficient credits.");

        var content = await _aiProvider.ShortenBlogAsync(
            blog.Content,
            request?.Instructions);

        if (string.IsNullOrWhiteSpace(content))
            throw new InvalidOperationException(
                "AI provider returned empty content.");

        blog.Content = content;
        blog.Excerpt = CreateExcerpt(content);
        blog.WordCount = CountWords(content);
        blog.CreditsUsed += creditsRequired;
        blog.UpdatedAt = DateTime.UtcNow;

        user.AvailableCredits -= creditsRequired;

        _context.BlogVersions.Add(new BlogVersion
        {
            BlogId = blog.BlogId,
            Title = blog.Title,
            Content = content,
            WordCount = blog.WordCount,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return CreateResponse(
            blog,
            creditsRequired,
            user.AvailableCredits);
    }

    // =========================================================
    // 5. GENERATE IMAGE
    // =========================================================

    public async Task<GenerateImageResponseDto> GenerateImageAsync(
    int userId,
    int blogId,
    GenerateImageRequestDto? request)
    {
        var blog = await GetUserBlogAsync(userId, blogId);
        var user = await GetUserAsync(userId);

        const int creditsRequired = 10;

        if (user.AvailableCredits < creditsRequired)
            throw new InvalidOperationException(
                "Insufficient credits.");

        var prompt = string.IsNullOrWhiteSpace(request?.Prompt)
            ? $"Create a professional blog cover image for: {blog.Title}"
            : request.Prompt;

        // Generate a unique filename
        var fileName =
            $"blog_{blogId}_{Guid.NewGuid():N}.jpg";

        // Cloudflare returns Base64 image data
        var imageBase64 =
            await _aiProvider.GenerateImageAsync(prompt);

        if (string.IsNullOrWhiteSpace(imageBase64))
            throw new InvalidOperationException(
                "AI provider did not return an image.");

        // Upload Base64 image to Cloudinary
        var imageUrl = await _imageStorageService
     .UploadImageAsync(imageBase64, fileName);

        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new InvalidOperationException(
                "Image storage service did not return an image URL.");

        user.AvailableCredits -= creditsRequired;

        var image = new BlogImage
        {
            BlogId = blogId,
            Prompt = prompt,
            ImageUrl = imageUrl,
            CreditsUsed = creditsRequired,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow
        };

        _context.BlogImages.Add(image);

        await _context.SaveChangesAsync();

        return new GenerateImageResponseDto
        {
            BlogId = blogId,
            ImageUrl = imageUrl
        };
    }

    // =========================================================
    // 6. AI HISTORY
    // =========================================================

    public async Task<List<AIHistoryDto>> GetAIHistoryAsync(
        int userId,
        int blogId)
    {
        await GetUserBlogAsync(userId, blogId);

        return await _context.BlogVersions
            .Where(x => x.BlogId == blogId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new AIHistoryDto
            {
                BlogVersionId = x.VersionId,
                BlogId = x.BlogId,
                Content = x.Content,
                CreditsUsed = 0,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }

    // =========================================================
    // 7. REWRITE
    // =========================================================

    public async Task<GenerateBlogResponseDto> RewriteBlogAsync(
        int userId,
        int blogId,
        RewriteBlogRequestDto request)
    {
        if (request == null ||
            string.IsNullOrWhiteSpace(request.Instructions))
        {
            throw new ArgumentException(
                "Rewrite instructions are required.");
        }

        var blog = await GetUserBlogAsync(userId, blogId);
        var user = await GetUserAsync(userId);

        const int creditsRequired = 5;

        if (user.AvailableCredits < creditsRequired)
            throw new InvalidOperationException(
                "Insufficient credits.");

        var content = await _aiProvider.RewriteBlogAsync(
            blog.Content,
            request.Instructions);

        if (string.IsNullOrWhiteSpace(content))
            throw new InvalidOperationException(
                "AI provider returned empty content.");

        blog.Content = content;
        blog.Excerpt = CreateExcerpt(content);
        blog.WordCount = CountWords(content);
        blog.CreditsUsed += creditsRequired;
        blog.UpdatedAt = DateTime.UtcNow;

        user.AvailableCredits -= creditsRequired;

        _context.BlogVersions.Add(new BlogVersion
        {
            BlogId = blogId,
            Title = blog.Title,
            Content = content,
            WordCount = blog.WordCount,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return CreateResponse(
            blog,
            creditsRequired,
            user.AvailableCredits);
    }

    // =========================================================
    // 8. TRANSLATE
    // =========================================================

    public async Task<GenerateBlogResponseDto> TranslateBlogAsync(
    int userId,
    int blogId,
    TranslateBlogRequestDto request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var blog = await GetUserBlogAsync(userId, blogId);
        var user = await GetUserAsync(userId);

        const int creditsRequired = 5;

        if (user.AvailableCredits < creditsRequired)
            throw new InvalidOperationException(
                "Insufficient credits.");

        var content = await _aiProvider.TranslateBlogAsync(
            blog.Content,
            request.Language.ToString());

        if (string.IsNullOrWhiteSpace(content))
            throw new InvalidOperationException(
                "AI provider returned empty content.");

        blog.Content = content;
        blog.Excerpt = CreateExcerpt(content);
        blog.WordCount = CountWords(content);
        blog.CreditsUsed += creditsRequired;
        blog.UpdatedAt = DateTime.UtcNow;

        blog.Language = request.Language;

        user.AvailableCredits -= creditsRequired;

        _context.BlogVersions.Add(new BlogVersion
        {
            BlogId = blogId,
            Title = blog.Title,
            Content = content,
            WordCount = blog.WordCount,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return CreateResponse(
            blog,
            creditsRequired,
            user.AvailableCredits);
    }

    // =========================================================
    // 9. GENERATE TAGS
    // =========================================================

    public async Task<GenerateTagsResponseDto> GenerateTagsAsync(
        int userId,
        int blogId)
    {
        var blog = await GetUserBlogAsync(userId, blogId);
        var user = await GetUserAsync(userId);

        const int creditsRequired = 2;

        if (user.AvailableCredits < creditsRequired)
            throw new InvalidOperationException(
                "Insufficient credits.");

        var result = await _aiProvider.GenerateTagsAsync(
            blog.Content);

        if (string.IsNullOrWhiteSpace(result))
            throw new InvalidOperationException(
                "AI provider returned no tags.");

        // Parse and clean tags
        var tagNames = result
            .Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (!tagNames.Any())
            throw new InvalidOperationException(
                "No valid tags were generated.");

        // Remove existing tags associated with this blog
        var existingBlogTags = await _context.BlogTags
            .Where(x => x.BlogId == blogId)
            .ToListAsync();

        if (existingBlogTags.Any())
        {
            _context.BlogTags.RemoveRange(existingBlogTags);
        }

        // Add generated tags
        foreach (var tagName in tagNames)
        {
            var tag = await _context.Tags
                .FirstOrDefaultAsync(x =>
                    x.Name.ToLower() == tagName.ToLower());

            if (tag == null)
            {
                tag = new Tags
                {
                    Name = tagName,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Tags.Add(tag);

                // Save so TagId is generated
                await _context.SaveChangesAsync();
            }

            var blogTag = new BlogTags
            {
                BlogId = blogId,
                TagId = tag.TagId
            };

            _context.BlogTags.Add(blogTag);
        }

        user.AvailableCredits -= creditsRequired;

        await _context.SaveChangesAsync();

        return new GenerateTagsResponseDto
        {
            BlogId = blogId,
            Tags = tagNames
        };
    }

    // =========================================================
    // 10. GENERATE SUMMARY
    // =========================================================

    public async Task<GenerateSummaryResponseDto> GenerateSummaryAsync(
        int userId,
        int blogId)
    {
        var blog = await GetUserBlogAsync(userId, blogId);
        var user = await GetUserAsync(userId);

        const int creditsRequired = 2;

        if (user.AvailableCredits < creditsRequired)
            throw new InvalidOperationException(
                "Insufficient credits.");

        var summary = await _aiProvider.GenerateSummaryAsync(
            blog.Content);

        if (string.IsNullOrWhiteSpace(summary))
            throw new InvalidOperationException(
                "AI provider returned empty summary.");

        blog.Excerpt = CreateExcerpt(summary);
        blog.UpdatedAt = DateTime.UtcNow;

        user.AvailableCredits -= creditsRequired;

        await _context.SaveChangesAsync();

        return new GenerateSummaryResponseDto
        {
            BlogId = blogId,
            Summary = summary
        };
    }

    // =========================================================
    // 11. GET TAGS
    // =========================================================

    public async Task<List<TagDto>> GetTagsAsync(
    int userId,
    int blogId)
    {
        // Verify that the blog belongs to the logged-in user
        await GetUserBlogAsync(userId, blogId);

        var tags = await _context.BlogTags
            .Where(x => x.BlogId == blogId)
            .Select(x => new TagDto
            {
                TagId = x.Tag.TagId,
                Name = x.Tag.Name
            })
            .OrderBy(x => x.Name)
            .ToListAsync();

        return tags;
    }

    // =========================================================
    // 12. PUBLISH BLOG
    // =========================================================

    public async Task<bool> PublishBlogAsync(
      int userId,
      int blogId)
    {
        var blog = await GetUserBlogAsync(userId, blogId);

        // Already published
        if (blog.Status == BlogStatus.Published)
            throw new InvalidOperationException(
                "Blog is already published.");

        // Already waiting for admin
        if (blog.Status == BlogStatus.PendingApproval)
            throw new InvalidOperationException(
                "Blog is already pending admin approval.");

        // Rejected blogs cannot be published again
        if (blog.Status == BlogStatus.Rejected)
            throw new InvalidOperationException(
                "This blog was rejected and cannot be published.");

        // ---------------------------------------------------------
        // Check Trusted badge
        // ---------------------------------------------------------

        var isTrusted = await _context.UserBadges
            .AnyAsync(x =>
                x.UserId == userId &&
                x.Badge.Name == "Trusted");

        if (isTrusted)
        {
            // Trusted users can publish directly.
            blog.Status = BlogStatus.Published;
            blog.Visibility = BlogVisibility.Public;
            blog.PublishedAt = DateTime.UtcNow;
            blog.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Trusted user {UserId} published blog {BlogId}.",
                userId,
                blogId);

            return true;
        }

        // ---------------------------------------------------------
        // Count user's approved/published blogs
        // ---------------------------------------------------------

        var approvedBlogCount = await _context.Blogs
            .CountAsync(x =>
                x.UserId == userId &&
                x.Status == BlogStatus.Published);

        // ---------------------------------------------------------
        // First 3 blogs require admin approval
        // ---------------------------------------------------------

        if (approvedBlogCount < 3)
        {
            blog.Status = BlogStatus.PendingApproval;

            // It must not appear publicly while waiting.
            blog.Visibility = BlogVisibility.Private;

            blog.PublishedAt = null;
            blog.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Blog {BlogId} submitted for admin approval by user {UserId}. " +
                "Approved blogs: {ApprovedCount}",
                blogId,
                userId,
                approvedBlogCount);

            return true;
        }

        /*
         * IMPORTANT:
         *
         * At this point the user has 3 approved blogs,
         * but if the Trusted badge has not yet been awarded,
         * this should normally be handled by the admin approval
         * process of the third blog.
         *
         * We do NOT award the badge here.
         */

        throw new InvalidOperationException(
            "Your account is eligible for Trusted status, " +
            "but the Trusted badge has not yet been assigned. " +
            "Please contact an administrator.");
    }

    // =========================================================
    // HELPER METHODS
    // =========================================================

    private async Task<User> GetUserAsync(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                !x.IsDeleted &&
                x.IsActive);

        if (user == null)
            throw new KeyNotFoundException(
                "User not found.");

        return user;
    }

    private async Task<BlogEntity> GetUserBlogAsync(
        int userId,
        int blogId)
    {
        var blog = await _context.Blogs
            .FirstOrDefaultAsync(x =>
                x.BlogId == blogId &&
                x.UserId == userId);

        if (blog == null)
            throw new KeyNotFoundException(
                "Blog not found or you do not have permission to access it.");

        return blog;
    }

    private static GenerateBlogResponseDto CreateResponse(
        BlogEntity blog,
        int creditsUsed,
        int remainingCredits)
    {
        return new GenerateBlogResponseDto
        {
            BlogId = blog.BlogId,
            Title = blog.Title,
            Content = blog.Content,
            Excerpt = blog.Excerpt,
            CreditsUsed = creditsUsed,
            RemainingCredits = remainingCredits
        };
    }

    private static string BuildGenerationPrompt(
        GenerateBlogRequestDto request)
    {
        return $"""
            Write a high-quality blog post.

            Topic: {request.Topic}
            Target Audience: {request.Audience}
            Tone: {request.Tone}
            Word Count: approximately {request.WordCount} words.
            Language: {request.Language}

            Create engaging, original and well-structured content.
            Include an appropriate introduction, meaningful sections,
            and a strong conclusion.
            """;
    }

    private static string CreateExcerpt(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return string.Empty;

        const int maxLength = 250;

        return content.Length <= maxLength
            ? content
            : content[..maxLength].Trim() + "...";
    }

    private static int CountWords(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return 0;

        return content
            .Split(
                Array.Empty<char>(),
                StringSplitOptions.RemoveEmptyEntries)
            .Length;
    }

    private static string GenerateSlug(string title)
    {
        return string.Join(
            "-",
            title
                .ToLowerInvariant()
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries));
    }


}