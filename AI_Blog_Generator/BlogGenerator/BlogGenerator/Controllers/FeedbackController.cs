using BlogGenerator.Interfaces;
using BlogGenerator.ServiceModels.v1.Feedback;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogGenerator.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _service;

        public FeedbackController(
            IFeedbackService service)
        {
            _service = service;
        }

        private int CurrentUserId =>
            int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!.Value);

        // =========================================================
        // SUBMIT FEEDBACK
        // POST /api/feedback
        // =========================================================

        [HttpPost]
        public async Task<IActionResult> SubmitFeedback(
            [FromBody] CreateFeedbackDto dto)
        {
            var result =
                await _service.SubmitFeedbackAsync(
                    CurrentUserId,
                    dto);

            return Ok(result);
        }

        // =========================================================
        // GET MY FEEDBACK
        // GET /api/feedback
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> GetMyFeedback()
        {
            var result =
                await _service.GetMyFeedbackAsync(
                    CurrentUserId);

            return Ok(result);
        }

        // =========================================================
        // GET FEEDBACK BY ID
        // GET /api/feedback/{id}
        // =========================================================

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeedbackById(int id)
        {
            var result =
                await _service.GetFeedbackByIdAsync(
                    id,
                    CurrentUserId);

            if (result == null)
                return NotFound("Feedback not found.");

            return Ok(result);
        }

        // =========================================================
        // GET PUBLIC FEEDBACK
        // GET /api/feedback/public
        // =========================================================

        [AllowAnonymous]
        [HttpGet("public")]
        public async Task<IActionResult> GetPublicFeedback()
        {
            var result =
                await _service.GetPublicFeedbackAsync();

            return Ok(result);
        }
    }
}