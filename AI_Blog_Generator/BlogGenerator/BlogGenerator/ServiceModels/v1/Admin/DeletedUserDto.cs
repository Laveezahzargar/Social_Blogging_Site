


namespace BlogGenerator.ServiceModels.v1;


public class DeletedUserDto
{
    public int DeletedAccountId { get; set; }
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTime DeletedAt { get; set; }
}