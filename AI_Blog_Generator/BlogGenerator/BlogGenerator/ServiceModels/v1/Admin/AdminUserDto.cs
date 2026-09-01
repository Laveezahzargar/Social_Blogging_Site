


namespace BlogGenerator.ServiceModels.v1;

public class AdminUserDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public int AvailableCredits { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime CreatedAt { get; set; }
}