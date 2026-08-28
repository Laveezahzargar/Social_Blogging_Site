using BlogGenerator.ServiceModels.v1.BlogInteraction;

namespace BlogGenerator.Interfaces
{
    public interface IBlogInteractionService
    {
        // =====================================================
        // BLOG LIKES
        // =====================================================

        Task LikeBlogAsync(int blogId, int userId);

        Task UnlikeBlogAsync(int blogId, int userId);

        Task<List<BlogLikeUserDto>> GetBlogLikesAsync(
            int blogId);

        // =====================================================
        // COMMENTS
        // =====================================================

        Task<CommentDto> AddCommentAsync(
            int blogId,
            int userId,
            CreateCommentDto dto);

        Task<List<CommentDto>> GetCommentsAsync(
            int blogId,
            int? currentUserId = null);

        Task<CommentDto> UpdateCommentAsync(
            int commentId,
            int userId,
            UpdateCommentDto dto);

        Task DeleteCommentAsync(
            int commentId,
            int userId);

        // =====================================================
        // SAVE
        // =====================================================

        Task SaveBlogAsync(
            int blogId,
            int userId);

        Task UnsaveBlogAsync(
            int blogId,
            int userId);

        // =====================================================
        // REPOST
        // =====================================================

        Task RepostBlogAsync(
            int blogId,
            int userId);

        Task RemoveRepostAsync(
            int blogId,
            int userId);

        // =====================================================
        // REPORT
        // =====================================================

        Task ReportBlogAsync(
            int blogId,
            int userId,
            ReportBlogDto dto);

        // =====================================================
        // COMMENT LIKES
        // =====================================================

        Task LikeCommentAsync(
            int commentId,
            int userId);

        Task UnlikeCommentAsync(
            int commentId,
            int userId);
    }
}