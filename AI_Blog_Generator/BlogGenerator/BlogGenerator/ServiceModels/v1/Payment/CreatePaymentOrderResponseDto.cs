namespace BlogGenerator.ServiceModels.v1;

public class CreatePaymentOrderResponseDto
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public int PaymentId { get; set; }

    public string RazorpayKeyId { get; set; } = string.Empty;

    public string RazorpayOrderId { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "INR";
}