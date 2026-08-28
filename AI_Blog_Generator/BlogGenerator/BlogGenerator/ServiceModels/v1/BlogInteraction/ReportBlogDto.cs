using BlogGenerator.Enums;

namespace BlogGenerator.ServiceModels.v1.BlogInteraction
{
    public class ReportBlogDto
    {
        public ReportReason Reason { get; set; } 
        public string? Description { get; set; }
    }
}
