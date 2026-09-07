
using BlogGenerator.Enums;

namespace BlogGenerator.ServiceModels.v1.AIBlog;

public class GenerateBlogRequestDto
{
    public int CategoryId { get; set; }

    public string Topic { get; set; } = string.Empty;

    public BlogAudience Audience { get; set; } = BlogAudience.General;

    public BlogTone Tone { get; set; } = BlogTone.Professional;

    public BlogWordCount WordCount { get; set; } = BlogWordCount.Words500;

    public BlogLanguage Language { get; set; } = BlogLanguage.English;
}