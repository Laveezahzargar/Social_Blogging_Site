using Azure;
using BlogGenerator.Configurations;
using BlogGenerator.DomainModels.v1;
using Microsoft.EntityFrameworkCore;
using static QuestPDF.Helpers.Colors;

namespace BlogGenerator.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }
        public DbSet<User> Users => Set<User>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public DbSet<Blog> Blogs => Set<Blog>();

        public DbSet<BlogVersion> BlogVersions => Set<BlogVersion>();

        public DbSet<BlogImage> BlogImages => Set<BlogImage>();

        public DbSet<Plan> Plans => Set<Plan>();

        public DbSet<Payment> Payments => Set<Payment>();

        public DbSet<Feedback> Feedbacks => Set<Feedback>();

        public DbSet<Issue> Issues => Set<Issue>();

        public DbSet<DeletedAccount> DeletedAccounts => Set<DeletedAccount>();

        public DbSet<Likes> Likes => Set<Likes>();

        public DbSet<Comments> Comments => Set<Comments>();

        public DbSet<Bookmarks> Bookmarks => Set<Bookmarks>();

        public DbSet<Reposts> Reposts => Set<Reposts>();

        public DbSet<Follow> Follows => Set<Follow>();

        public DbSet<Notifications> Notifications => Set<Notifications>();

        public DbSet<BlogReports> BlogReports => Set<BlogReports>();

        public DbSet<UserBadges> UserBadges => Set<UserBadges>();
        public DbSet<CommentLikes> CommentLikes => Set<CommentLikes>();
        public DbSet<Category> Categories => Set<Category>();

        public DbSet<Tags> Tags => Set<Tags>();

        public DbSet<BlogTags> BlogTags => Set<BlogTags>();

        public DbSet<Badges> Badges => Set<Badges>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

    }
}
