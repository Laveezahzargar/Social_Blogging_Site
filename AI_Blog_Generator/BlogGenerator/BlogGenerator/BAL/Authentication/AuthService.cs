using BlogGenerator.DAL;
using BlogGenerator.DomainModels.v1;
using BlogGenerator.Enums;
using BlogGenerator.Foundation.Exceptions;
using BlogGenerator.Interfaces.Authentication;
using BlogGenerator.ServiceModels.v1.Authentication;
using Microsoft.EntityFrameworkCore;

namespace BlogGenerator.BAL.Authentication;


public class AuthService : IAuthService
{

    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ApplicationDbContext context,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        ILogger<AuthService> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
        _logger = logger;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var emailExists = await _context.Users
            .AnyAsync(x => x.Email == request.Email);

        if (emailExists)
        {
            _logger.LogWarning(
        "Registration failed. Email {Email} already exists.",
        request.Email);
            throw new ConflictException("Email is already registered.");
        }

        var userNameExists = await _context.Users
            .AnyAsync(x => x.UserName == request.UserName);

        if (userNameExists)
        {
            _logger.LogWarning(
        "Registration failed. Username {UserName} already exists.",
        request.UserName);
            throw new ConflictException("Username already exists.");
        }

        _logger.LogInformation(
    "Registering user {Email}",
    request.Email);

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.User,
            AvailableCredits = 100,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        _logger.LogInformation(
    "User {UserId} registered successfully.",
    user.UserId);

        var accessToken = _jwtService.GenerateAccessToken(user);

        var refreshToken = _refreshTokenService.GenerateRefreshToken();

        await _refreshTokenService.SaveRefreshTokenAsync(
            user,
            refreshToken
        );

        return new AuthResponseDto
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Email = user.Email,
            Role = user.Role.ToString(),
            AvailableCredits = user.AvailableCredits,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = _jwtService.GetAccessTokenExpiry()
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null)
        {
            _logger.LogWarning(
    "Invalid login attempt for {Email}",
    request.Email);
            throw new UnauthorizedException("Invalid email or password.");
        }

        if (user.IsDeleted)
        {
            _logger.LogWarning(
    "Invalid login attempt for deleted account with {Email}",
    request.Email);
            throw new UnauthorizedException("Invalid email or password.");
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash);

        if (!isPasswordValid)
        {
            _logger.LogWarning(
    "Invalid login attempt for {Email}",
    request.Email);
            throw new UnauthorizedException("Invalid email or password.");
        }

        var accessToken = _jwtService.GenerateAccessToken(user);

        var refreshToken = _refreshTokenService.GenerateRefreshToken();

        await _refreshTokenService.SaveRefreshTokenAsync(
            user,
            refreshToken);
        _logger.LogInformation(
    "User {UserId} logged in.",
    user.UserId);
        return new AuthResponseDto
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Email = user.Email,
            Role = user.Role.ToString(),
            AvailableCredits = user.AvailableCredits,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = _jwtService.GetAccessTokenExpiry()
        };
        
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var existingToken = await _refreshTokenService
            .GetRefreshTokenAsync(request.RefreshToken);

        if (existingToken == null)
        {
            _logger.LogWarning("Invalid refresh token received.");
            throw new UnauthorizedException("Refresh token is invalid.");
        }

        if (existingToken.IsRevoked)
        {
            _logger.LogWarning("Revoked refresh token was used.");
            throw new UnauthorizedException("Refresh token has been revoked.");
        }

        if (existingToken.ExpiryDate <= DateTime.UtcNow)
        {
            _logger.LogWarning("Expired refresh token was used.");
            throw new BadRequestException("Refresh token has expired.");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.UserId == existingToken.UserId);

        if (user == null)
        {
            _logger.LogWarning("Refresh token belongs to a non-existent user.");
            throw new NotFoundException("User not found.");
        }

        if (user.IsDeleted)
        {
            _logger.LogWarning(
        "Deleted user {UserId} attempted token refresh.",
        user.UserId);
            throw new UnauthorizedException("User account has been deleted.");
        }

        var accessToken = _jwtService.GenerateAccessToken(user);

        var newRefreshToken = _refreshTokenService.GenerateRefreshToken();

        await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken);

        await _refreshTokenService.SaveRefreshTokenAsync(
            user,
            newRefreshToken);

        _logger.LogInformation(
   "Refresh token generated for User {UserId}",
   user.UserId);

        return new AuthResponseDto
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Email = user.Email,
            Role = user.Role.ToString(),
            AvailableCredits = user.AvailableCredits,
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = _jwtService.GetAccessTokenExpiry()
        };
       
    }

    public async Task<AuthResponseDto> GenerateGuestTokenAsync()
    {
        var guestUser = await _context.Users
            .FirstOrDefaultAsync(x => x.Role == UserRole.Guest);

        if (guestUser == null)
        {
            _logger.LogError("Guest user was not found in the database.");

            throw new NotFoundException(
                "Guest user is not configured.");
        }

        if (guestUser.IsDeleted || !guestUser.IsActive)
        {
            _logger.LogError(
                "Guest user is inactive or deleted. UserId: {UserId}",
                guestUser.UserId);

            throw new UnauthorizedException(
                "Guest access is currently unavailable.");
        }

        var accessToken = _jwtService.GenerateAccessToken(guestUser);

        _logger.LogInformation(
            "Guest access token generated. UserId: {UserId}",
            guestUser.UserId);

        return new AuthResponseDto
        {
            UserId = guestUser.UserId,
            UserName = guestUser.UserName,
            Email = guestUser.Email,
            Role = guestUser.Role.ToString(),
            AvailableCredits = guestUser.AvailableCredits,
            AccessToken = accessToken,
            RefreshToken = string.Empty,
            ExpiresAt = _jwtService.GetAccessTokenExpiry()
        };
    }

    public async Task LogoutAsync(LogoutRequestDto request)
    {
        var refreshToken = await _refreshTokenService
            .GetRefreshTokenAsync(request.RefreshToken);

        if (refreshToken == null)
        {
            _logger.LogWarning("Logout attempted with an invalid refresh token.");
            throw new UnauthorizedException("Refresh token not found.");
        }

        if (refreshToken.IsRevoked)
        {
            _logger.LogInformation(
        "Logout requested for an already revoked refresh token. UserId: {UserId}",
        refreshToken.UserId);
            return;
        }

        await _refreshTokenService
            .RevokeRefreshTokenAsync(request.RefreshToken);

        _logger.LogInformation(
    "User {UserId} logged out successfully.",
    refreshToken.UserId);
    }
}