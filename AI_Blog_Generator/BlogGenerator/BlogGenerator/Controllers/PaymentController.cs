using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlogGenerator.ServiceModels.v1;
using BlogGenerator.Interfaces;
using System.Security.Claims;

namespace BlogGenerator.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    // =========================================================
    // CREATE RAZORPAY ORDER
    // POST /api/v1/payment/create-order
    // =========================================================

    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder(
        CreatePaymentOrderRequestDto request)
    {
        var userId = GetUserId();

        var result =
            await _paymentService.CreatePaymentOrderAsync(
                userId,
                request);

        return Ok(result);
    }


    // =========================================================
    // VERIFY RAZORPAY PAYMENT
    // POST /api/v1/payment/verify
    // =========================================================

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyPayment(
        ConfirmPaymentRequestDto request)
    {
        var userId = GetUserId();

        var result =
            await _paymentService.ConfirmPaymentAsync(
                userId,
                request);

        return Ok(result);
    }


    // =========================================================
    // PAYMENT HISTORY
    // GET /api/v1/payment/history
    // =========================================================

    [HttpGet("history")]
    public async Task<IActionResult> GetPaymentHistory()
    {
        var userId = GetUserId();

        var result =
            await _paymentService.GetPaymentHistoryAsync(
                userId);

        return Ok(result);
    }


    // =========================================================
    // PAYMENT DETAILS
    // GET /api/v1/payment/{paymentId}
    // =========================================================

    [HttpGet("{paymentId:int}")]
    public async Task<IActionResult> GetPayment(
        int paymentId)
    {
        var userId = GetUserId();

        var result =
            await _paymentService.GetPaymentByIdAsync(
                userId,
                paymentId);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Payment not found."
            });
        }

        return Ok(result);
    }


    // =========================================================
    // CANCEL PAYMENT
    // POST /api/v1/payment/{paymentId}/cancel
    // =========================================================

    [HttpPost("{paymentId:int}/cancel")]
    public async Task<IActionResult> CancelPayment(
        int paymentId)
    {
        var userId = GetUserId();

        var result =
            await _paymentService.CancelPaymentAsync(
                userId,
                paymentId);

        return Ok(result);
    }


    // =========================================================
    // RAZORPAY WEBHOOK
    // POST /api/v1/payment/webhook
    // =========================================================

    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        using var reader =
            new StreamReader(Request.Body);

        var json =
            await reader.ReadToEndAsync();

        var signature =
            Request.Headers["X-Razorpay-Signature"]
                .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(signature))
        {
            return BadRequest(
                "Razorpay signature is missing.");
        }

        await _paymentService.HandleWebhookAsync(
            json,
            signature);

        return Ok();
    }


    // =========================================================
    // GET USER ID FROM JWT
    // =========================================================

    private int GetUserId()
    {
        var claim =
            User.FindFirst(
                ClaimTypes.NameIdentifier);

        if (claim == null)
        {
            throw new UnauthorizedAccessException(
                "User ID not found in token.");
        }

        return int.Parse(claim.Value);
    }
}