

namespace BlogGenerator.ServiceModels.v1;


public class AdminBlogDetailsDto : AdminBlogDto
{
    public string Content { get; set; } = string.Empty;
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Reposts { get; set; }
}