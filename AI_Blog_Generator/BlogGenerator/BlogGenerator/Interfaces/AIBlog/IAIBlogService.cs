using BlogGenerator.ServiceModels.v1.AIBlog;

namespace BlogGenerator.Interfaces;

public interface IAIBlogService
{
    Task<GenerateBlogResponseDto> GenerateBlogAsync(
        int userId,
        GenerateBlogRequestDto request);

    Task<GenerateBlogResponseDto> RegenerateBlogAsync(
        int userId,
        int blogId);

    Task<GenerateBlogResponseDto> ExpandBlogAsync(
        int userId,
        int blogId,
        BlogActionRequestDto? request);

    Task<GenerateBlogResponseDto> ShortenBlogAsync(
        int userId,
        int blogId,
        BlogActionRequestDto? request);

    Task<GenerateImageResponseDto> GenerateImageAsync(
        int userId,
        int blogId,
        GenerateImageRequestDto? request);

    Task<List<AIHistoryDto>> GetAIHistoryAsync(
        int userId,
        int blogId);

    Task<GenerateBlogResponseDto> RewriteBlogAsync(
        int userId,
        int blogId,
        RewriteBlogRequestDto request);

    Task<GenerateBlogResponseDto> TranslateBlogAsync(
        int userId,
        int blogId,
        TranslateBlogRequestDto request);

    Task<GenerateTagsResponseDto> GenerateTagsAsync(
        int userId,
        int blogId);

    Task<GenerateSummaryResponseDto> GenerateSummaryAsync(
        int userId,
        int blogId);

    Task<List<TagDto>> GetTagsAsync(
        int userId,
        int blogId);
    Task<bool> PublishBlogAsync(
        int userId,
        int blogId);
}