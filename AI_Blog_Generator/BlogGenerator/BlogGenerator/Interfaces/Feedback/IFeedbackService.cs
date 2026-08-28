using BlogGenerator.ServiceModels.v1.Feedback;

namespace BlogGenerator.Interfaces
{
    public interface IFeedbackService
    {
        Task<FeedbackDto> SubmitFeedbackAsync(
            int userId,
            CreateFeedbackDto dto);

        Task<List<FeedbackDto>> GetMyFeedbackAsync(
            int userId);

        Task<FeedbackDto?> GetFeedbackByIdAsync(
            int feedbackId,
            int userId);

        Task<List<FeedbackDto>> GetPublicFeedbackAsync();
    }
}