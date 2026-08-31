using BlogGenerator.ServiceModels.v1;

namespace BlogGenerator.Interfaces;

public interface IPaymentService
{
    Task<CreatePaymentOrderResponseDto> CreatePaymentOrderAsync(
        int userId,
        CreatePaymentOrderRequestDto request);

    Task<PaymentResponseDto> ConfirmPaymentAsync(
        int userId,
        ConfirmPaymentRequestDto request);

    Task<List<PaymentHistoryDto>> GetPaymentHistoryAsync(
        int userId);

    Task<PaymentResponseDto?> GetPaymentByIdAsync(
        int userId,
        int paymentId);

    Task<PaymentResponseDto> CancelPaymentAsync(
        int userId,
        int paymentId);

    Task HandleWebhookAsync(
        string json,
        string razorpaySignature);
}