using System.ComponentModel.DataAnnotations;

namespace BlogGenerator.ServiceModels.v1.Issue
{
    public class CreateIssueDto
    {
        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;
    }
}