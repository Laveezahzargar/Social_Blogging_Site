using BlogGenerator.ServiceModels.v1.PublicFeed;

namespace BlogGenerator.Interfaces;

public interface IPublicFeedService
{
    Task<List<FeedBlogDto>> GetFeedAsync();

    Task<List<FeedBlogDto>> GetTrendingAsync();

    Task<List<FeedBlogDto>> GetFollowingFeedAsync(
        int userId);

    Task<List<FeedBlogDto>> GetByCategoryAsync(
        string category);

    Task<List<FeedBlogDto>> SearchAsync(
        string search);

    Task<FeedBlogDetailsDto> GetBlogAsync(
        int blogId);
}