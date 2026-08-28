using BlogGenerator.Enums;

namespace BlogGenerator.ServiceModels.v1.Issue
{
    public class IssueDto
    {
        public int IssueId { get; set; }

        public int UserId { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public IssueStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}