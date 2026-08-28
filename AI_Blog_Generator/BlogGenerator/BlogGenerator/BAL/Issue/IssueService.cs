using BlogGenerator.DAL;
using BlogGenerator.ServiceModels.v1.Issue;
using BlogGenerator.Interfaces;
using IssueEntity = BlogGenerator.DomainModels.v1.Issue;
using Microsoft.EntityFrameworkCore;

namespace BlogGenerator.BAL.Issue
{
    public class IssueService : IIssueService
    {
        private readonly ApplicationDbContext _context;

        public IssueService(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // CREATE ISSUE
        // POST /api/issues
        // =========================================================

        public async Task<IssueDto> CreateIssueAsync(
            int userId,
            CreateIssueDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Subject))
                throw new ArgumentException(
                    "Subject is required.");

            if (string.IsNullOrWhiteSpace(dto.Description))
                throw new ArgumentException(
                    "Description is required.");

            var issue = new IssueEntity
            {
                UserId = userId,
                Subject = dto.Subject,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            _context.Issues.Add(issue);

            await _context.SaveChangesAsync();

            return MapToDto(issue);
        }

        // =========================================================
        // GET MY ISSUES
        // GET /api/issues
        // =========================================================

        public async Task<List<IssueDto>> GetMyIssuesAsync(
            int userId)
        {
            return await _context.Issues
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.CreatedAt)
                .Select(i => new IssueDto
                {
                    IssueId = i.IssueId,
                    UserId = i.UserId,
                    Subject = i.Subject,
                    Description = i.Description,
                    Status = i.Status,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                })
                .ToListAsync();
        }

        // =========================================================
        // GET ISSUE BY ID
        // GET /api/issues/{id}
        // =========================================================

        public async Task<IssueDto?> GetIssueByIdAsync(
            int issueId,
            int userId)
        {
            return await _context.Issues
                .Where(i =>
                    i.IssueId == issueId &&
                    i.UserId == userId)
                .Select(i => new IssueDto
                {
                    IssueId = i.IssueId,
                    UserId = i.UserId,
                    Subject = i.Subject,
                    Description = i.Description,
                    Status = i.Status,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        // =========================================================
        // UPDATE ISSUE
        // PUT /api/issues/{id}
        // =========================================================

        public async Task<IssueDto?> UpdateIssueAsync(
            int issueId,
            int userId,
            UpdateIssueDto dto)
        {
            var issue = await _context.Issues
                .FirstOrDefaultAsync(i =>
                    i.IssueId == issueId &&
                    i.UserId == userId);

            if (issue == null)
                return null;

            if (string.IsNullOrWhiteSpace(dto.Subject))
                throw new ArgumentException(
                    "Subject is required.");

            if (string.IsNullOrWhiteSpace(dto.Description))
                throw new ArgumentException(
                    "Description is required.");

            // User can update only before resolution.
            if (issue.Status.ToString()
                .Equals("Resolved",
                    StringComparison.OrdinalIgnoreCase)
                ||
                issue.Status.ToString()
                .Equals("Closed",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "A resolved or closed issue cannot be updated.");
            }

            issue.Subject = dto.Subject;
            issue.Description = dto.Description;
            issue.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToDto(issue);
        }

        // =========================================================
        // DELETE / WITHDRAW ISSUE
        // DELETE /api/issues/{id}
        // =========================================================

        public async Task<bool> DeleteIssueAsync(
            int issueId,
            int userId)
        {
            var issue = await _context.Issues
                .FirstOrDefaultAsync(i =>
                    i.IssueId == issueId &&
                    i.UserId == userId);

            if (issue == null)
                return false;

            if (issue.Status.ToString()
                .Equals("Resolved",
                    StringComparison.OrdinalIgnoreCase)
                ||
                issue.Status.ToString()
                .Equals("Closed",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "A resolved or closed issue cannot be withdrawn.");
            }

            _context.Issues.Remove(issue);

            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // GET ISSUE STATUS
        // GET /api/issues/{id}/status
        // =========================================================

        public async Task<IssueDto?> GetIssueStatusAsync(
            int issueId,
            int userId)
        {
            return await _context.Issues
                .Where(i =>
                    i.IssueId == issueId &&
                    i.UserId == userId)
                .Select(i => new IssueDto
                {
                    IssueId = i.IssueId,
                    UserId = i.UserId,
                    Subject = i.Subject,
                    Description = i.Description,
                    Status = i.Status,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        // =========================================================
        // MAPPER
        // =========================================================

        private static IssueDto MapToDto(
            IssueEntity issue)
        {
            return new IssueDto
            {
                IssueId = issue.IssueId,
                UserId = issue.UserId,
                Subject = issue.Subject,
                Description = issue.Description,
                Status = issue.Status,
                CreatedAt = issue.CreatedAt,
                UpdatedAt = issue.UpdatedAt
            };
        }
    }
}