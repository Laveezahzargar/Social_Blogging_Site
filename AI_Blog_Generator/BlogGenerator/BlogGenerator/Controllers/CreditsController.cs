using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BlogGenerator.ServiceModels.v1;
using BlogGenerator.Interfaces;
using BlogGenerator.ServiceModels.v1.Foundation;

namespace BlogGenerator.Controllers;

[ApiController]
[Route("api/v1/[Controller]")]
[Authorize]
public class CreditsController : ControllerBase
{
    private readonly ICreditService _creditService;

    public CreditsController(ICreditService creditService)
    {
        _creditService = creditService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAvailableCredits()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _creditService.GetAvailableCreditsAsync(userId);

        return Ok(new ApiResponse<GetCreditsResponseDto>
        {
            Success = true,
            Message = "Available credits retrieved successfully.",
            Data = result
        });
    }

    [HttpGet("pricing")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPricing()
    {
        var result = await _creditService.GetPricingAsync();

        return Ok(new ApiResponse<List<CreditPricingResponseDto>>
        {
            Success = true,
            Message = "Credit pricing retrieved successfully.",
            Data = result
        });
    }
}