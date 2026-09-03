
using BlogGenerator.Enums;

namespace BlogGenerator.ServiceModels.v1.AIBlog;

public class GenerateBlogRequestDto
{
    public int CategoryId { get; set; }

    public string Topic { get; set; } = string.Empty;

    public BlogAudience Audience { get; set; } 

    public BlogTone Tone { get; set; } 

    public BlogWordCount WordCount { get; set; }

    public BlogLanguage Language { get; set; } = BlogLanguage.English;
}