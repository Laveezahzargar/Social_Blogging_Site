using BlogGenerator.ServiceModels.v1.Authentication;

namespace BlogGenerator.Interfaces.Authentication;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);

    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);

    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task<AuthResponseDto> GenerateGuestTokenAsync();

    Task LogoutAsync(LogoutRequestDto request);
    
}