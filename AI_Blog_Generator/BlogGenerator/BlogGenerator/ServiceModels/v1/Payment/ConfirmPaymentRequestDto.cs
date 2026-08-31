namespace BlogGenerator.ServiceModels.v1;

public class ConfirmPaymentRequestDto
{
    public int PaymentId { get; set; }

    public string RazorpayOrderId { get; set; } = string.Empty;

    public string RazorpayPaymentId { get; set; } = string.Empty;

    public string RazorpaySignature { get; set; } = string.Empty;
}