using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using BlogGenerator.DAL;
using BlogGenerator.DomainModels.v1;
using BlogGenerator.ServiceModels.v1;
using BlogGenerator.Enums;
using BlogGenerator.Interfaces;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using PaymentEntity = BlogGenerator.DomainModels.v1.Payment;

namespace BlogGenerator.Services;

public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public PaymentService(
        ApplicationDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // =========================================================
    // CREATE RAZORPAY ORDER
    // POST /api/payments/create-order
    // =========================================================

    public async Task<CreatePaymentOrderResponseDto>
        CreatePaymentOrderAsync(
            int userId,
            CreatePaymentOrderRequestDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                !x.IsDeleted &&
                x.IsActive);

        if (user == null)
            throw new Exception("User not found.");

        var plan = await _context.Plans
            .FirstOrDefaultAsync(x =>
                x.PlanId == request.PlanId &&
                x.IsActive);

        if (plan == null)
            throw new Exception(
                "Plan not found or inactive.");

        if (plan.Price <= 0)
            throw new Exception(
                "Invalid plan price.");

        if (plan.Credits <= 0)
            throw new Exception(
                "Invalid plan credits.");

        var keyId =
            _configuration["Razorpay:KeyId"];

        var keySecret =
            _configuration["Razorpay:KeySecret"];

        if (string.IsNullOrWhiteSpace(keyId) ||
            string.IsNullOrWhiteSpace(keySecret))
        {
            throw new Exception(
                "Razorpay configuration is missing.");
        }

        // Razorpay expects amount in paise.
        var amountInPaise =
            (int)(plan.Price * 100);

        var client = new RazorpayClient(
            keyId,
            keySecret);

        var options =
            new Dictionary<string, object>
            {
                { "amount", amountInPaise },
                { "currency", "INR" },
                {
                    "receipt",
                    $"receipt_{userId}_{DateTime.UtcNow.Ticks}"
                }
            };

        var order =
            client.Order.Create(options);

        var razorpayOrderId =
            order["id"].ToString();

        if (string.IsNullOrWhiteSpace(
            razorpayOrderId))
        {
            throw new Exception(
                "Failed to create Razorpay order.");
        }

        // -----------------------------------------------------
        // Create local payment.
        // Credits are NOT added here.
        // -----------------------------------------------------

        var payment = new PaymentEntity
        {
            UserId = userId,

            PlanId = plan.PlanId,

            Amount = plan.Price,

            CreditsPurchased =
                plan.Credits,

            RazorpayOrderId =
                razorpayOrderId,

            PaymentStatus =
                PaymentStatus.Pending,

            PurchasedAt =
                DateTime.UtcNow
        };

        _context.Payments.Add(payment);

        await _context.SaveChangesAsync();

        return new CreatePaymentOrderResponseDto
        {
            Success = true,

            Message =
                "Razorpay order created successfully.",

            PaymentId =
                payment.PaymentId,

            RazorpayKeyId =
                keyId,

            RazorpayOrderId =
                razorpayOrderId,

            Amount =
                plan.Price,

            Currency =
                "INR"
        };
    }


    // =========================================================
    // CONFIRM PAYMENT
    // POST /api/payments/confirm
    // =========================================================

    public async Task<PaymentResponseDto>
        ConfirmPaymentAsync(
            int userId,
            ConfirmPaymentRequestDto request)
    {
        var payment =
            await _context.Payments
                .FirstOrDefaultAsync(x =>
                    x.PaymentId ==
                    request.PaymentId &&
                    x.UserId ==
                    userId);

        if (payment == null)
            throw new Exception(
                "Payment not found.");

        // Prevent duplicate credits.
        if (payment.PaymentStatus ==
            PaymentStatus.Completed)
        {
            return MapPayment(payment);
        }

        if (string.IsNullOrWhiteSpace(
            request.RazorpayOrderId))
        {
            throw new Exception(
                "Razorpay order ID is required.");
        }

        if (string.IsNullOrWhiteSpace(
            request.RazorpayPaymentId))
        {
            throw new Exception(
                "Razorpay payment ID is required.");
        }

        if (string.IsNullOrWhiteSpace(
            request.RazorpaySignature))
        {
            throw new Exception(
                "Razorpay signature is required.");
        }

        // Make sure the order belongs to
        // this local payment.
        if (payment.RazorpayOrderId !=
            request.RazorpayOrderId)
        {
            throw new Exception(
                "Razorpay order does not match the payment.");
        }

        var keyId =
            _configuration["Razorpay:KeyId"];

        var keySecret =
            _configuration["Razorpay:KeySecret"];

        if (string.IsNullOrWhiteSpace(keyId) ||
            string.IsNullOrWhiteSpace(keySecret))
        {
            throw new Exception(
                "Razorpay configuration is missing.");
        }

        // -----------------------------------------------------
        // Verify Razorpay checkout signature
        // -----------------------------------------------------

        var attributes =
            new Dictionary<string, string>
            {
                {
                    "razorpay_order_id",
                    request.RazorpayOrderId
                },
                {
                    "razorpay_payment_id",
                    request.RazorpayPaymentId
                },
                {
                    "razorpay_signature",
                    request.RazorpaySignature
                }
            };

        Utils.verifyPaymentSignature(attributes);

        var user =
            await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.UserId ==
                    userId &&
                    !x.IsDeleted &&
                    x.IsActive);

        if (user == null)
            throw new Exception(
                "User not found.");

        // -----------------------------------------------------
        // Payment successful
        // -----------------------------------------------------

        payment.RazorpayPaymentId =
            request.RazorpayPaymentId;

        payment.RazorpaySignature =
            request.RazorpaySignature;

        payment.PaymentStatus =
            PaymentStatus.Completed;

        // Add credits exactly once.
        user.AvailableCredits +=
            payment.CreditsPurchased;

        await _context.SaveChangesAsync();

        return MapPayment(payment);
    }


    // =========================================================
    // PAYMENT HISTORY
    // GET /api/payments/history
    // =========================================================

    public async Task<List<PaymentHistoryDto>>
        GetPaymentHistoryAsync(int userId)
    {
        return await _context.Payments
            .Where(x =>
                x.UserId == userId)
            .OrderByDescending(x =>
                x.PurchasedAt)
            .Select(x =>
                new PaymentHistoryDto
                {
                    PaymentId =
                        x.PaymentId,

                    PlanId =
                        x.PlanId,

                    Amount =
                        x.Amount,

                    CreditsPurchased =
                        x.CreditsPurchased,

                    RazorpayOrderId =
                        x.RazorpayOrderId,

                    RazorpayPaymentId =
                        x.RazorpayPaymentId,

                    PaymentStatus =
                        x.PaymentStatus,

                    PurchasedAt =
                        x.PurchasedAt
                })
            .ToListAsync();
    }


    // =========================================================
    // GET PAYMENT DETAILS
    // GET /api/payments/{paymentId}
    // =========================================================

    public async Task<PaymentResponseDto?>
        GetPaymentByIdAsync(
            int userId,
            int paymentId)
    {
        var payment =
            await _context.Payments
                .FirstOrDefaultAsync(x =>
                    x.PaymentId ==
                    paymentId &&
                    x.UserId ==
                    userId);

        if (payment == null)
            return null;

        return MapPayment(payment);
    }


    // =========================================================
    // CANCEL PAYMENT
    // POST /api/payments/{paymentId}/cancel
    // =========================================================

    public async Task<PaymentResponseDto>
        CancelPaymentAsync(
            int userId,
            int paymentId)
    {
        var payment =
            await _context.Payments
                .FirstOrDefaultAsync(x =>
                    x.PaymentId ==
                    paymentId &&
                    x.UserId ==
                    userId);

        if (payment == null)
            throw new Exception(
                "Payment not found.");

        if (payment.PaymentStatus !=
            PaymentStatus.Pending)
        {
            throw new Exception(
                "Only pending payments can be cancelled.");
        }

        // Razorpay does not use the same
        // PaymentIntent cancellation model
        // as Stripe.
        //
        // We simply cancel the local pending
        // payment/order.

        payment.PaymentStatus =
            PaymentStatus.Cancelled;

        await _context.SaveChangesAsync();

        return MapPayment(payment);
    }


    // =========================================================
    // RAZORPAY WEBHOOK
    // POST /api/payments/webhook
    // =========================================================

    public async Task HandleWebhookAsync(
        string json,
        string razorpaySignature)
    {
        var webhookSecret =
            _configuration[
                "Razorpay:WebhookSecret"];

        if (string.IsNullOrWhiteSpace(
            webhookSecret))
        {
            throw new Exception(
                "Razorpay webhook secret is missing.");
        }

        if (string.IsNullOrWhiteSpace(
            razorpaySignature))
        {
            throw new Exception(
                "Razorpay webhook signature is missing.");
        }

        // -----------------------------------------------------
        // Verify webhook signature
        // -----------------------------------------------------

        VerifyWebhookSignature(
            json,
            razorpaySignature,
            webhookSecret);

        using var document =
            JsonDocument.Parse(json);

        var root =
            document.RootElement;

        if (!root.TryGetProperty(
            "event",
            out var eventProperty))
        {
            return;
        }

        var eventName =
            eventProperty.GetString();

        // =====================================================
        // PAYMENT CAPTURED
        // =====================================================

        if (eventName ==
            "payment.captured")
        {
            if (!root.TryGetProperty(
                "payload",
                out var payload))
            {
                return;
            }

            if (!payload.TryGetProperty(
                "payment",
                out var paymentPayload))
            {
                return;
            }

            if (!paymentPayload.TryGetProperty(
                "entity",
                out var paymentEntity))
            {
                return;
            }

            var razorpayPaymentId =
                paymentEntity
                    .GetProperty("id")
                    .GetString();

            var razorpayOrderId =
                paymentEntity
                    .GetProperty("order_id")
                    .GetString();

            if (string.IsNullOrWhiteSpace(
                razorpayPaymentId) ||
                string.IsNullOrWhiteSpace(
                razorpayOrderId))
            {
                return;
            }

            var payment =
                await _context.Payments
                    .FirstOrDefaultAsync(x =>
                        x.RazorpayOrderId ==
                        razorpayOrderId);

            if (payment == null)
                return;

            // Prevent duplicate credits.
            if (payment.PaymentStatus ==
                PaymentStatus.Completed)
            {
                return;
            }

            var user =
                await _context.Users
                    .FirstOrDefaultAsync(x =>
                        x.UserId ==
                        payment.UserId &&
                        !x.IsDeleted &&
                        x.IsActive);

            if (user == null)
                return;

            payment.RazorpayPaymentId =
                razorpayPaymentId;

            payment.PaymentStatus =
                PaymentStatus.Completed;

            user.AvailableCredits +=
                payment.CreditsPurchased;

            await _context.SaveChangesAsync();
        }


        // =====================================================
        // PAYMENT FAILED
        // =====================================================

        else if (eventName ==
                 "payment.failed")
        {
            if (!root.TryGetProperty(
                "payload",
                out var payload))
            {
                return;
            }

            if (!payload.TryGetProperty(
                "payment",
                out var paymentPayload))
            {
                return;
            }

            if (!paymentPayload.TryGetProperty(
                "entity",
                out var paymentEntity))
            {
                return;
            }

            if (!paymentEntity.TryGetProperty(
                "order_id",
                out var orderProperty))
            {
                return;
            }

            var razorpayOrderId =
                orderProperty.GetString();

            if (string.IsNullOrWhiteSpace(
                razorpayOrderId))
            {
                return;
            }

            var payment =
                await _context.Payments
                    .FirstOrDefaultAsync(x =>
                        x.RazorpayOrderId ==
                        razorpayOrderId);

            if (payment == null)
                return;

            if (payment.PaymentStatus ==
                PaymentStatus.Pending)
            {
                payment.PaymentStatus =
                    PaymentStatus.Failed;

                await _context.SaveChangesAsync();
            }
        }
    }


    // =========================================================
    // WEBHOOK SIGNATURE VERIFICATION
    // =========================================================

    private static void VerifyWebhookSignature(
        string payload,
        string signature,
        string secret)
    {
        using var hmac =
            new HMACSHA256(
                Encoding.UTF8.GetBytes(secret));

        var hash =
            hmac.ComputeHash(
                Encoding.UTF8.GetBytes(payload));

        var expectedSignature =
            Convert.ToHexString(hash)
                .ToLowerInvariant();

        if (!CryptographicOperations
            .FixedTimeEquals(
                Encoding.UTF8.GetBytes(
                    expectedSignature),
                Encoding.UTF8.GetBytes(
                    signature)))
        {
            throw new Exception(
                "Invalid Razorpay webhook signature.");
        }
    }


    // =========================================================
    // MAPPER
    // =========================================================

    private static PaymentResponseDto
        MapPayment(PaymentEntity payment)
    {
        return new PaymentResponseDto
        {
            PaymentId =
                payment.PaymentId,

            UserId =
                payment.UserId,

            PlanId =
                payment.PlanId,

            Amount =
                payment.Amount,

            CreditsPurchased =
                payment.CreditsPurchased,

            RazorpayOrderId =
                payment.RazorpayOrderId,

            RazorpayPaymentId =
                payment.RazorpayPaymentId,

            PaymentStatus =
                payment.PaymentStatus,

            PurchasedAt =
                payment.PurchasedAt
        };
    }
}