using BlogGenerator.DomainModels.v1;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogGenerator.Configurations;

public class BlogTagConfiguration : IEntityTypeConfiguration<BlogTags>
{
    public void Configure(EntityTypeBuilder<BlogTags> builder)
    {
        builder.ToTable("BlogTags");

        builder.HasKey(x => x.BlogTagId);

        builder.Property(x => x.BlogId)
            .IsRequired();

        builder.Property(x => x.TagId)
            .IsRequired();

        builder.HasOne(x => x.Blog)
            .WithMany()
            .HasForeignKey(x => x.BlogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Tag)
    .WithMany(x => x.BlogTags)
    .HasForeignKey(x => x.TagId)
    .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.BlogId, x.TagId })
            .IsUnique();
    }
}