using BlogGenerator.Enums;

namespace BlogGenerator.ServiceModels.v1;

public class PaymentHistoryDto
{
    public int PaymentId { get; set; }

    public int PlanId { get; set; }

    public decimal Amount { get; set; }

    public int CreditsPurchased { get; set; }

    public string RazorpayOrderId { get; set; } = string.Empty;

    public string? RazorpayPaymentId { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public DateTime PurchasedAt { get; set; }
}