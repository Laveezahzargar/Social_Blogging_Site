using BlogGenerator.DomainModels.v1;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogGenerator.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comments>
{
    public void Configure(EntityTypeBuilder<Comments> builder)
    {
        builder.ToTable("Comments");

        builder.HasKey(x => x.CommentId);

        builder.Property(x => x.BlogId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.ParentCommentId);

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.IsEdited)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.HasOne(x => x.Blog)
    .WithMany(x => x.Comments)
    .HasForeignKey(x => x.BlogId)
    .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ParentComment)
    .WithMany(x => x.Replies)
    .HasForeignKey(x => x.ParentCommentId)
    .OnDelete(DeleteBehavior.Restrict);
    }
}