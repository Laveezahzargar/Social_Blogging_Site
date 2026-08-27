using BlogGenerator.Interfaces.Authentication;
using BlogGenerator.ServiceModels.v1.Foundation;
using BlogGenerator.ServiceModels.v1.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogGenerator.Controllers
{
    [ApiController]
    [Route("api/v1/[Controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterAsync(request);

            return StatusCode(StatusCodes.Status201Created,
                new ApiResponse<AuthResponseDto>
                {
                    Success = true,
                    Message = "User registered successfully.",
                    Data = result
                });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);

            return Ok(new ApiResponse<AuthResponseDto>
            {
                Success = true,
                Message = "Login successful.",
                Data = result
            });
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshTokenAsync(request);

            return Ok(new ApiResponse<AuthResponseDto>
            {
                Success = true,
                Message = "Token refreshed successfully.",
                Data = result
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request)
        {
            await _authService.LogoutAsync(request);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Logged out successfully.",
                Data = null
            });
        }
    }
}