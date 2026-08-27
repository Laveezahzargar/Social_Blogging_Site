using BlogGenerator.DomainModels.v1;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogGenerator.Configurations;

public class RepostConfiguration : IEntityTypeConfiguration<Reposts>
{
    public void Configure(EntityTypeBuilder<Reposts> builder)
    {
        builder.ToTable("Reposts");

        builder.HasKey(x => x.RepostId);

        builder.Property(x => x.BlogId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Caption)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Blog)
    .WithMany(x => x.Reposts)
    .HasForeignKey(x => x.BlogId)
    .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.BlogId, x.UserId })
            .IsUnique();
    }
}