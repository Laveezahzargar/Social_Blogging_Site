
using BlogGenerator.Enums;

namespace BlogGenerator.ServiceModels.v1.AIBlog;

public class TranslateBlogRequestDto
{
    public BlogLanguage Language { get; set; } = BlogLanguage.English;
}