

namespace BlogGenerator.ServiceModels.v1;


public class AdminPaymentDto
{
    public int PaymentId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string RazorpayOrderId { get; set; } = string.Empty;
    public string RazorpayPaymentId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}