using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogGenerator.Migrations
{
    /// <inheritdoc />
    public partial class Mig_003 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogReports_Blogs_BlogId1",
                table: "BlogReports");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogTags_Tags_TagsTagId",
                table: "BlogTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_Blogs_BlogId1",
                table: "Bookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Blogs_BlogId1",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_CommentsCommentId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Blogs_BlogId1",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Reposts_Blogs_BlogId1",
                table: "Reposts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBadges_Badges_BadgesBadgeId",
                table: "UserBadges");

            migrationBuilder.DropIndex(
                name: "IX_UserBadges_BadgesBadgeId",
                table: "UserBadges");

            migrationBuilder.DropIndex(
                name: "IX_Reposts_BlogId1",
                table: "Reposts");

            migrationBuilder.DropIndex(
                name: "IX_Likes_BlogId1",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Comments_BlogId1",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CommentsCommentId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Bookmarks_BlogId1",
                table: "Bookmarks");

            migrationBuilder.DropIndex(
                name: "IX_BlogTags_TagsTagId",
                table: "BlogTags");

            migrationBuilder.DropIndex(
                name: "IX_BlogReports_BlogId1",
                table: "BlogReports");

            migrationBuilder.DropColumn(
                name: "BadgesBadgeId",
                table: "UserBadges");

            migrationBuilder.DropColumn(
                name: "BlogId1",
                table: "Reposts");

            migrationBuilder.DropColumn(
                name: "BlogId1",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "BlogId1",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "CommentsCommentId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "BlogId1",
                table: "Bookmarks");

            migrationBuilder.DropColumn(
                name: "TagsTagId",
                table: "BlogTags");

            migrationBuilder.DropColumn(
                name: "BlogId1",
                table: "BlogReports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BadgesBadgeId",
                table: "UserBadges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BlogId1",
                table: "Reposts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BlogId1",
                table: "Likes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BlogId1",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CommentsCommentId",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BlogId1",
                table: "Bookmarks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TagsTagId",
                table: "BlogTags",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BlogId1",
                table: "BlogReports",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBadges_BadgesBadgeId",
                table: "UserBadges",
                column: "BadgesBadgeId");

            migrationBuilder.CreateIndex(
                name: "IX_Reposts_BlogId1",
                table: "Reposts",
                column: "BlogId1");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_BlogId1",
                table: "Likes",
                column: "BlogId1");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_BlogId1",
                table: "Comments",
                column: "BlogId1");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentsCommentId",
                table: "Comments",
                column: "CommentsCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookmarks_BlogId1",
                table: "Bookmarks",
                column: "BlogId1");

            migrationBuilder.CreateIndex(
                name: "IX_BlogTags_TagsTagId",
                table: "BlogTags",
                column: "TagsTagId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogReports_BlogId1",
                table: "BlogReports",
                column: "BlogId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogReports_Blogs_BlogId1",
                table: "BlogReports",
                column: "BlogId1",
                principalTable: "Blogs",
                principalColumn: "BlogId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogTags_Tags_TagsTagId",
                table: "BlogTags",
                column: "TagsTagId",
                principalTable: "Tags",
                principalColumn: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_Blogs_BlogId1",
                table: "Bookmarks",
                column: "BlogId1",
                principalTable: "Blogs",
                principalColumn: "BlogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Blogs_BlogId1",
                table: "Comments",
                column: "BlogId1",
                principalTable: "Blogs",
                principalColumn: "BlogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_CommentsCommentId",
                table: "Comments",
                column: "CommentsCommentId",
                principalTable: "Comments",
                principalColumn: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Blogs_BlogId1",
                table: "Likes",
                column: "BlogId1",
                principalTable: "Blogs",
                principalColumn: "BlogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reposts_Blogs_BlogId1",
                table: "Reposts",
                column: "BlogId1",
                principalTable: "Blogs",
                principalColumn: "BlogId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBadges_Badges_BadgesBadgeId",
                table: "UserBadges",
                column: "BadgesBadgeId",
                principalTable: "Badges",
                principalColumn: "BadgeId");
        }
    }
}
