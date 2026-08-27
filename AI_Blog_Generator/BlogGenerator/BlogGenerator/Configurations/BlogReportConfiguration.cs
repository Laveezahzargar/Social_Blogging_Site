using BlogGenerator.DomainModels.v1;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogGenerator.Configurations;

public class BlogReportConfiguration : IEntityTypeConfiguration<BlogReports>
{
    public void Configure(EntityTypeBuilder<BlogReports> builder)
    {
        builder.ToTable("BlogReports");

        builder.HasKey(x => x.ReportId);

        builder.Property(x => x.BlogId)
            .IsRequired();

        builder.Property(x => x.ReportedByUserId)
            .IsRequired();

        builder.Property(x => x.Reason)
            .IsRequired();

        builder.Property(x => x.Description);

        builder.Property(x => x.ReportStatus)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Blog)
    .WithMany(x => x.BlogReports)
    .HasForeignKey(x => x.BlogId)
    .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ReportedByUser)
            .WithMany()
            .HasForeignKey(x => x.ReportedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}