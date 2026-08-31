using BlogGenerator.DomainModels.v1;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogGenerator.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(x => x.PaymentId);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.PlanId)
            .IsRequired();

        builder.Property(x => x.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.CreditsPurchased)
            .IsRequired();

        // =====================================================
        // RAZORPAY FIELDS
        // =====================================================

        builder.Property(x => x.RazorpayOrderId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.RazorpayPaymentId)
            .IsRequired(false)
            .HasMaxLength(255);

        builder.Property(x => x.RazorpaySignature)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(x => x.PaymentStatus)
            .IsRequired();

        builder.Property(x => x.PurchasedAt)
            .IsRequired();

        // =====================================================
        // USER → PAYMENTS
        // =====================================================

        builder.HasOne(x => x.User)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // =====================================================
        // PLAN → PAYMENTS
        // =====================================================

        builder.HasOne(x => x.Plan)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}