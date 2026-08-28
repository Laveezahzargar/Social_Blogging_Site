using BlogGenerator.Interfaces;
using BlogGenerator.ServiceModels.v1.Issue;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogGenerator.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class IssueController : ControllerBase
    {
        private readonly IIssueService _service;

        public IssueController(IIssueService service)
        {
            _service = service;
        }

        private int CurrentUserId =>
            int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!.Value);

        // =========================================================
        // CREATE ISSUE
        // POST /api/issues
        // =========================================================

        [HttpPost]
        public async Task<IActionResult> CreateIssue(
            [FromBody] CreateIssueDto dto)
        {
            var result =
                await _service.CreateIssueAsync(
                    CurrentUserId,
                    dto);

            return Ok(result);
        }

        // =========================================================
        // GET MY ISSUES
        // GET /api/issues
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> GetMyIssues()
        {
            var result =
                await _service.GetMyIssuesAsync(
                    CurrentUserId);

            return Ok(result);
        }

        // =========================================================
        // GET ISSUE BY ID
        // GET /api/issues/{id}
        // =========================================================

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIssueById(
            int id)
        {
            var result =
                await _service.GetIssueByIdAsync(
                    id,
                    CurrentUserId);

            if (result == null)
                return NotFound(
                    "Issue not found.");

            return Ok(result);
        }

        // =========================================================
        // UPDATE ISSUE
        // PUT /api/issues/{id}
        // =========================================================

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIssue(
            int id,
            [FromBody] UpdateIssueDto dto)
        {
            var result =
                await _service.UpdateIssueAsync(
                    id,
                    CurrentUserId,
                    dto);

            if (result == null)
                return NotFound(
                    "Issue not found.");

            return Ok(result);
        }

        // =========================================================
        // DELETE / WITHDRAW ISSUE
        // DELETE /api/issues/{id}
        // =========================================================

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIssue(
            int id)
        {
            var result =
                await _service.DeleteIssueAsync(
                    id,
                    CurrentUserId);

            if (!result)
                return NotFound(
                    "Issue not found.");

            return Ok(new
            {
                success = true,
                message = "Issue withdrawn successfully."
            });
        }

        // =========================================================
        // GET ISSUE STATUS
        // GET /api/issues/{id}/status
        // =========================================================

        [HttpGet("{id}/status")]
        public async Task<IActionResult> GetIssueStatus(
            int id)
        {
            var result =
                await _service.GetIssueStatusAsync(
                    id,
                    CurrentUserId);

            if (result == null)
                return NotFound(
                    "Issue not found.");

            return Ok(new
            {
                issueId = result.IssueId,
                status = result.Status
            });
        }
    }
}