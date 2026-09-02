using System.ComponentModel.DataAnnotations;

namespace BlogGenerator.ServiceModels.v1.Authentication;

public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}