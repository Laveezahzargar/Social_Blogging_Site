using BlogGenerator.DomainModels.v1;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogGenerator.Configurations;

public class UserBadgeConfiguration : IEntityTypeConfiguration<UserBadges>
{
    public void Configure(EntityTypeBuilder<UserBadges> builder)
    {
        builder.ToTable("UserBadges");

        builder.HasKey(x => x.UserBadgeId);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.BadgeId)
            .IsRequired();

        builder.Property(x => x.EarnedAt)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Badge)
    .WithMany(x => x.UserBadges)
    .HasForeignKey(x => x.BadgeId)
    .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.UserId, x.BadgeId })
            .IsUnique();
    }
}