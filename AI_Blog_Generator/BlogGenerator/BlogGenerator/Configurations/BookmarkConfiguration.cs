using BlogGenerator.DomainModels.v1;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogGenerator.Configurations;

public class BookmarkConfiguration : IEntityTypeConfiguration<Bookmarks>
{
    public void Configure(EntityTypeBuilder<Bookmarks> builder)
    {
        builder.ToTable("Bookmarks");

        builder.HasKey(x => x.BookmarkId);

        builder.Property(x => x.BlogId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Blog)
    .WithMany(x => x.Bookmarks)
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