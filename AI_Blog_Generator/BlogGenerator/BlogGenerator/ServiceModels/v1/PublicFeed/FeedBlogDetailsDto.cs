
namespace BlogGenerator.ServiceModels.v1.PublicFeed;

public class FeedBlogDetailsDto
{
    public int BlogId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Excerpt { get; set; } = string.Empty;

    public string? CoverImageUrl { get; set; }

    public int UserId { get; set; }

    public string AuthorName { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public int WordCount { get; set; }

    public DateTime? PublishedAt { get; set; }

    public int ViewCount { get; set; }

    public int LikeCount { get; set; }

    public int CommentCount { get; set; }

    public List<string> Tags { get; set; } = new();
}