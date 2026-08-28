using BlogGenerator.DAL;
using BlogGenerator.DomainModels.v1;
using BlogGenerator.Interfaces;
using BlogGenerator.ServiceModels.v1.Feedback;
using FeedbackEntity = BlogGenerator.DomainModels.v1.Feedback;
using Microsoft.EntityFrameworkCore;

namespace BlogGenerator.BAL.Feedback
{
    public class FeedbackService : IFeedbackService
    {
        private readonly ApplicationDbContext _context;

        public FeedbackService(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // SUBMIT FEEDBACK
        // POST /api/feedback
        // =========================================================

        public async Task<FeedbackDto> SubmitFeedbackAsync(
            int userId,
            CreateFeedbackDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Subject))
                throw new ArgumentException(
                    "Subject is required.");

            if (string.IsNullOrWhiteSpace(dto.Message))
                throw new ArgumentException(
                    "Message is required.");

            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ArgumentException(
                    "Rating must be between 1 and 5.");

            var feedback = new FeedbackEntity
            {
                UserId = userId,
                Subject = dto.Subject,
                Message = dto.Message,
                Rating = dto.Rating,
                IsPublic = dto.IsPublic,
                CreatedAt = DateTime.UtcNow
            };

            _context.Feedbacks.Add(feedback);

            await _context.SaveChangesAsync();

            return MapToDto(feedback);
        }

        // =========================================================
        // GET MY FEEDBACK
        // GET /api/feedback
        // =========================================================

        public async Task<List<FeedbackDto>> GetMyFeedbackAsync(
            int userId)
        {
            return await _context.Feedbacks
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new FeedbackDto
                {
                    FeedbackId = f.FeedbackId,
                    UserId = f.UserId,
                    Subject = f.Subject,
                    Message = f.Message,
                    Rating = f.Rating,
                    IsPublic = f.IsPublic,
                    Status = f.Status,
                    AdminResponse = f.AdminResponse,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                })
                .ToListAsync();
        }

        // =========================================================
        // GET FEEDBACK BY ID
        // GET /api/feedback/{id}
        // =========================================================

        public async Task<FeedbackDto?> GetFeedbackByIdAsync(
            int feedbackId,
            int userId)
        {
            return await _context.Feedbacks
                .Where(f =>
                    f.FeedbackId == feedbackId &&
                    f.UserId == userId)
                .Select(f => new FeedbackDto
                {
                    FeedbackId = f.FeedbackId,
                    UserId = f.UserId,
                    Subject = f.Subject,
                    Message = f.Message,
                    Rating = f.Rating,
                    IsPublic = f.IsPublic,
                    Status = f.Status,
                    AdminResponse = f.AdminResponse,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        // =========================================================
        // GET PUBLIC FEEDBACK
        // GET /api/feedback/public
        // =========================================================

        public async Task<List<FeedbackDto>> GetPublicFeedbackAsync()
        {
            return await _context.Feedbacks
                .Where(f => f.IsPublic)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new FeedbackDto
                {
                    FeedbackId = f.FeedbackId,
                    UserId = f.UserId,
                    Subject = f.Subject,
                    Message = f.Message,
                    Rating = f.Rating,
                    IsPublic = f.IsPublic,
                    Status = f.Status,
                    AdminResponse = f.AdminResponse,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                })
                .ToListAsync();
        }

        private static FeedbackDto MapToDto(
            FeedbackEntity feedback)
        {
            return new FeedbackDto
            {
                FeedbackId = feedback.FeedbackId,
                UserId = feedback.UserId,
                Subject = feedback.Subject,
                Message = feedback.Message,
                Rating = feedback.Rating,
                IsPublic = feedback.IsPublic,
                Status = feedback.Status,
                AdminResponse = feedback.AdminResponse,
                CreatedAt = feedback.CreatedAt,
                UpdatedAt = feedback.UpdatedAt
            };
        }
    }
}