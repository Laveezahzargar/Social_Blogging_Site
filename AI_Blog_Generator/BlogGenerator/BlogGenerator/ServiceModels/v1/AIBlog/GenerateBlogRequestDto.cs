
using BlogGenerator.Enums;

namespace BlogGenerator.ServiceModels.v1.AIBlog;

public class GenerateBlogRequestDto
{
    public int CategoryId { get; set; }

    public string Topic { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public string Tone { get; set; } = string.Empty;

    public int WordCount { get; set; }

    public BlogLanguage Language { get; set; } = BlogLanguage.English;
}