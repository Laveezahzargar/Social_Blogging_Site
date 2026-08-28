using System.ComponentModel.DataAnnotations;

namespace BlogGenerator.ServiceModels.v1.Feedback
{
    public class CreateFeedbackDto
    {
        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Rating { get; set; }

        public bool IsPublic { get; set; } = false;
    }
}