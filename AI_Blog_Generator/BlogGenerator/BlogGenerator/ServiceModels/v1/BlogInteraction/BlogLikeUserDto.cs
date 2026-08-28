namespace BlogGenerator.ServiceModels.v1.BlogInteraction
{
    public class BlogLikeUserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
    }
}
