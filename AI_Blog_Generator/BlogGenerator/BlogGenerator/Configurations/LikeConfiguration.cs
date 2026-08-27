using BlogGenerator.DomainModels.v1;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogGenerator.Configurations;

public class LikeConfiguration : IEntityTypeConfiguration<Likes>
{
    public void Configure(EntityTypeBuilder<Likes> builder)
    {
        builder.ToTable("Likes");

        builder.HasKey(x => x.LikeId);

        builder.Property(x => x.BlogId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Blog)
    .WithMany(x => x.Likes)
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