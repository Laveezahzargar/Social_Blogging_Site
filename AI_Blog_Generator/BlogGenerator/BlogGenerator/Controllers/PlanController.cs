using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlogGenerator.ServiceModels.v1.Foundation;
using BlogGenerator.ServiceModels.v1;
using BlogGenerator.Interfaces;

namespace BlogGenerator.Controllers;

[ApiController]
[Route("api/v1/[Controller]")]
[Authorize]
public class PlansController : ControllerBase
{
    private readonly IPlanService _planService;

    public PlansController(IPlanService planService)
    {
        _planService = planService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllPlans()
    {
        var result = await _planService.GetAllPlansAsync();

        return Ok(new ApiResponse<List<PlanResponseDto>>
        {
            Success = true,
            Message = "Plans retrieved successfully.",
            Data = result
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePlan([FromBody] CreatePlanRequestDto request)
    {
        var result = await _planService.CreatePlanAsync(request);

        return Ok(new ApiResponse<PlanResponseDto>
        {
            Success = true,
            Message = "Plan created successfully.",
            Data = result
        });
    }

    [HttpPut("{planId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePlan(int planId,[FromBody] UpdatePlanRequestDto request)
    {
        var result = await _planService.UpdatePlanAsync(planId, request);

        return Ok(new ApiResponse<PlanResponseDto>
        {
            Success = true,
            Message = "Plan updated successfully.",
            Data = result
        });
    }

    [HttpDelete("{planId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePlan(int planId)
    {
        await _planService.DeletePlanAsync(planId);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Plan deleted successfully."
        });
    }
}