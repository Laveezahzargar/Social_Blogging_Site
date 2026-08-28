namespace BlogGenerator.ServiceModels.v1.BlogInteraction
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public int BlogId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string Content { get; set; } = string.Empty;
        public int LikeCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}
