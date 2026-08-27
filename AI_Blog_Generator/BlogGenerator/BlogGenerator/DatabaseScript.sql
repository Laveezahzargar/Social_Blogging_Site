IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Badges] (
    [BadgeId] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NULL,
    [IconUrl] nvarchar(500) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Badges] PRIMARY KEY ([BadgeId])
);
GO

CREATE TABLE [Categories] (
    [CategoryId] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NULL,
    [Icon] nvarchar(500) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([CategoryId])
);
GO

CREATE TABLE [Plans] (
    [PlanId] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NULL,
    [Price] decimal(18,2) NOT NULL,
    [Credits] int NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Plans] PRIMARY KEY ([PlanId])
);
GO

CREATE TABLE [Tags] (
    [TagId] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Tags] PRIMARY KEY ([TagId])
);
GO

CREATE TABLE [Users] (
    [UserId] int NOT NULL IDENTITY,
    [UserName] nvarchar(100) NOT NULL,
    [Email] nvarchar(255) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [DisplayName] nvarchar(100) NULL,
    [Bio] nvarchar(500) NULL,
    [Website] nvarchar(255) NULL,
    [Location] nvarchar(150) NULL,
    [Role] int NOT NULL,
    [ProfilePictureUrl] nvarchar(500) NULL,
    [AvailableCredits] int NOT NULL DEFAULT 100,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [LastSeenAt] datetime2 NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
);
GO

CREATE TABLE [Blogs] (
    [BlogId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [CategoryId] int NOT NULL,
    [Title] nvarchar(255) NOT NULL,
    [Slug] nvarchar(255) NOT NULL,
    [Prompt] nvarchar(max) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [Excerpt] nvarchar(500) NULL,
    [Tone] nvarchar(max) NOT NULL,
    [Audience] nvarchar(max) NOT NULL,
    [WordCount] int NOT NULL,
    [CreditsUsed] int NOT NULL,
    [Language] int NOT NULL DEFAULT 0,
    [Status] int NOT NULL,
    [Visibility] int NOT NULL,
    [AllowComments] bit NOT NULL DEFAULT CAST(1 AS bit),
    [ReadingTime] int NULL,
    [ViewsCount] int NOT NULL DEFAULT 0,
    [LikesCount] int NOT NULL DEFAULT 0,
    [CommentsCount] int NOT NULL DEFAULT 0,
    [BookmarksCount] int NOT NULL DEFAULT 0,
    [RepostsCount] int NOT NULL DEFAULT 0,
    [PublishedAt] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Blogs] PRIMARY KEY ([BlogId]),
    CONSTRAINT [FK_Blogs_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([CategoryId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Blogs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [DeletedAccounts] (
    [DeletedId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [UserName] nvarchar(100) NOT NULL,
    [Email] nvarchar(255) NOT NULL,
    [Reason] nvarchar(500) NULL,
    [DeletedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_DeletedAccounts] PRIMARY KEY ([DeletedId]),
    CONSTRAINT [FK_DeletedAccounts_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Feedbacks] (
    [FeedbackId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Subject] nvarchar(255) NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [Rating] int NOT NULL,
    [IsPublic] bit NOT NULL DEFAULT CAST(0 AS bit),
    [Status] int NOT NULL,
    [AdminResponse] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Feedbacks] PRIMARY KEY ([FeedbackId]),
    CONSTRAINT [FK_Feedbacks_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Follows] (
    [FollowId] int NOT NULL IDENTITY,
    [FollowerUserId] int NOT NULL,
    [FollowingUserId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Follows] PRIMARY KEY ([FollowId]),
    CONSTRAINT [FK_Follows_Users_FollowerUserId] FOREIGN KEY ([FollowerUserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Follows_Users_FollowingUserId] FOREIGN KEY ([FollowingUserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Issues] (
    [IssueId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Subject] nvarchar(255) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Status] int NOT NULL,
    [AdminResponse] nvarchar(max) NULL,
    [ResolvedAt] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Issues] PRIMARY KEY ([IssueId]),
    CONSTRAINT [FK_Issues_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Payments] (
    [PaymentId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [PlanId] int NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [CreditsPurchased] int NOT NULL,
    [StripeTransactionId] nvarchar(255) NOT NULL,
    [PaymentStatus] int NOT NULL,
    [PurchasedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([PaymentId]),
    CONSTRAINT [FK_Payments_Plans_PlanId] FOREIGN KEY ([PlanId]) REFERENCES [Plans] ([PlanId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Payments_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [RefreshTokens] (
    [TokenId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Token] nvarchar(max) NOT NULL,
    [ExpiryDate] datetime2 NOT NULL,
    [IsRevoked] bit NOT NULL DEFAULT CAST(0 AS bit),
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([TokenId]),
    CONSTRAINT [FK_RefreshTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [UserBadges] (
    [UserBadgeId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [BadgeId] int NOT NULL,
    [EarnedAt] datetime2 NOT NULL,
    [BadgesBadgeId] int NULL,
    CONSTRAINT [PK_UserBadges] PRIMARY KEY ([UserBadgeId]),
    CONSTRAINT [FK_UserBadges_Badges_BadgeId] FOREIGN KEY ([BadgeId]) REFERENCES [Badges] ([BadgeId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_UserBadges_Badges_BadgesBadgeId] FOREIGN KEY ([BadgesBadgeId]) REFERENCES [Badges] ([BadgeId]),
    CONSTRAINT [FK_UserBadges_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [BlogImages] (
    [ImageId] int NOT NULL IDENTITY,
    [BlogId] int NOT NULL,
    [Prompt] nvarchar(max) NOT NULL,
    [ImageUrl] nvarchar(500) NOT NULL,
    [ImageType] int NOT NULL,
    [DisplayOrder] int NOT NULL DEFAULT 1,
    [CreditsUsed] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_BlogImages] PRIMARY KEY ([ImageId]),
    CONSTRAINT [FK_BlogImages_Blogs_BlogId] FOREIGN KEY ([BlogId]) REFERENCES [Blogs] ([BlogId]) ON DELETE CASCADE
);
GO

CREATE TABLE [BlogReports] (
    [ReportId] int NOT NULL IDENTITY,
    [BlogId] int NOT NULL,
    [ReportedByUserId] int NOT NULL,
    [Reason] int NOT NULL,
    [Description] nvarchar(max) NULL,
    [ReportStatus] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [BlogId1] int NULL,
    CONSTRAINT [PK_BlogReports] PRIMARY KEY ([ReportId]),
    CONSTRAINT [FK_BlogReports_Blogs_BlogId] FOREIGN KEY ([BlogId]) REFERENCES [Blogs] ([BlogId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_BlogReports_Blogs_BlogId1] FOREIGN KEY ([BlogId1]) REFERENCES [Blogs] ([BlogId]),
    CONSTRAINT [FK_BlogReports_Users_ReportedByUserId] FOREIGN KEY ([ReportedByUserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [BlogTags] (
    [BlogTagId] int NOT NULL IDENTITY,
    [BlogId] int NOT NULL,
    [TagId] int NOT NULL,
    [TagsTagId] int NULL,
    CONSTRAINT [PK_BlogTags] PRIMARY KEY ([BlogTagId]),
    CONSTRAINT [FK_BlogTags_Blogs_BlogId] FOREIGN KEY ([BlogId]) REFERENCES [Blogs] ([BlogId]) ON DELETE CASCADE,
    CONSTRAINT [FK_BlogTags_Tags_TagId] FOREIGN KEY ([TagId]) REFERENCES [Tags] ([TagId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_BlogTags_Tags_TagsTagId] FOREIGN KEY ([TagsTagId]) REFERENCES [Tags] ([TagId])
);
GO

CREATE TABLE [BlogVersions] (
    [VersionId] int NOT NULL IDENTITY,
    [BlogId] int NOT NULL,
    [Title] nvarchar(255) NOT NULL,
    [VersionType] int NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [WordCount] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_BlogVersions] PRIMARY KEY ([VersionId]),
    CONSTRAINT [FK_BlogVersions_Blogs_BlogId] FOREIGN KEY ([BlogId]) REFERENCES [Blogs] ([BlogId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Bookmarks] (
    [BookmarkId] int NOT NULL IDENTITY,
    [BlogId] int NOT NULL,
    [UserId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [BlogId1] int NULL,
    CONSTRAINT [PK_Bookmarks] PRIMARY KEY ([BookmarkId]),
    CONSTRAINT [FK_Bookmarks_Blogs_BlogId] FOREIGN KEY ([BlogId]) REFERENCES [Blogs] ([BlogId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Bookmarks_Blogs_BlogId1] FOREIGN KEY ([BlogId1]) REFERENCES [Blogs] ([BlogId]),
    CONSTRAINT [FK_Bookmarks_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Comments] (
    [CommentId] int NOT NULL IDENTITY,
    [BlogId] int NOT NULL,
    [UserId] int NOT NULL,
    [ParentCommentId] int NULL,
    [Content] nvarchar(max) NOT NULL,
    [IsEdited] bit NOT NULL DEFAULT CAST(0 AS bit),
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [BlogId1] int NULL,
    [CommentsCommentId] int NULL,
    CONSTRAINT [PK_Comments] PRIMARY KEY ([CommentId]),
    CONSTRAINT [FK_Comments_Blogs_BlogId] FOREIGN KEY ([BlogId]) REFERENCES [Blogs] ([BlogId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Comments_Blogs_BlogId1] FOREIGN KEY ([BlogId1]) REFERENCES [Blogs] ([BlogId]),
    CONSTRAINT [FK_Comments_Comments_CommentsCommentId] FOREIGN KEY ([CommentsCommentId]) REFERENCES [Comments] ([CommentId]),
    CONSTRAINT [FK_Comments_Comments_ParentCommentId] FOREIGN KEY ([ParentCommentId]) REFERENCES [Comments] ([CommentId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Comments_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Likes] (
    [LikeId] int NOT NULL IDENTITY,
    [BlogId] int NOT NULL,
    [UserId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [BlogId1] int NULL,
    CONSTRAINT [PK_Likes] PRIMARY KEY ([LikeId]),
    CONSTRAINT [FK_Likes_Blogs_BlogId] FOREIGN KEY ([BlogId]) REFERENCES [Blogs] ([BlogId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Likes_Blogs_BlogId1] FOREIGN KEY ([BlogId1]) REFERENCES [Blogs] ([BlogId]),
    CONSTRAINT [FK_Likes_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Reposts] (
    [RepostId] int NOT NULL IDENTITY,
    [BlogId] int NOT NULL,
    [UserId] int NOT NULL,
    [Caption] nvarchar(500) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [BlogId1] int NULL,
    CONSTRAINT [PK_Reposts] PRIMARY KEY ([RepostId]),
    CONSTRAINT [FK_Reposts_Blogs_BlogId] FOREIGN KEY ([BlogId]) REFERENCES [Blogs] ([BlogId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Reposts_Blogs_BlogId1] FOREIGN KEY ([BlogId1]) REFERENCES [Blogs] ([BlogId]),
    CONSTRAINT [FK_Reposts_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [CommentLikes] (
    [CommentLikeId] int NOT NULL IDENTITY,
    [CommentId] int NOT NULL,
    [UserId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_CommentLikes] PRIMARY KEY ([CommentLikeId]),
    CONSTRAINT [FK_CommentLikes_Comments_CommentId] FOREIGN KEY ([CommentId]) REFERENCES [Comments] ([CommentId]) ON DELETE CASCADE,
    CONSTRAINT [FK_CommentLikes_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Notifications] (
    [NotificationId] int NOT NULL IDENTITY,
    [ReceiverUserId] int NOT NULL,
    [SenderUserId] int NULL,
    [BlogId] int NULL,
    [CommentId] int NULL,
    [NotificationType] int NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [IsRead] bit NOT NULL DEFAULT CAST(0 AS bit),
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([NotificationId]),
    CONSTRAINT [FK_Notifications_Blogs_BlogId] FOREIGN KEY ([BlogId]) REFERENCES [Blogs] ([BlogId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Notifications_Comments_CommentId] FOREIGN KEY ([CommentId]) REFERENCES [Comments] ([CommentId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Notifications_Users_ReceiverUserId] FOREIGN KEY ([ReceiverUserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Notifications_Users_SenderUserId] FOREIGN KEY ([SenderUserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_Badges_Name] ON [Badges] ([Name]);
GO

CREATE INDEX [IX_BlogImages_BlogId] ON [BlogImages] ([BlogId]);
GO

CREATE INDEX [IX_BlogReports_BlogId] ON [BlogReports] ([BlogId]);
GO

CREATE INDEX [IX_BlogReports_BlogId1] ON [BlogReports] ([BlogId1]);
GO

CREATE INDEX [IX_BlogReports_ReportedByUserId] ON [BlogReports] ([ReportedByUserId]);
GO

CREATE INDEX [IX_Blogs_CategoryId] ON [Blogs] ([CategoryId]);
GO

CREATE UNIQUE INDEX [IX_Blogs_Slug] ON [Blogs] ([Slug]);
GO

CREATE INDEX [IX_Blogs_UserId] ON [Blogs] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_BlogTags_BlogId_TagId] ON [BlogTags] ([BlogId], [TagId]);
GO

CREATE INDEX [IX_BlogTags_TagId] ON [BlogTags] ([TagId]);
GO

CREATE INDEX [IX_BlogTags_TagsTagId] ON [BlogTags] ([TagsTagId]);
GO

CREATE INDEX [IX_BlogVersions_BlogId] ON [BlogVersions] ([BlogId]);
GO

CREATE UNIQUE INDEX [IX_Bookmarks_BlogId_UserId] ON [Bookmarks] ([BlogId], [UserId]);
GO

CREATE INDEX [IX_Bookmarks_BlogId1] ON [Bookmarks] ([BlogId1]);
GO

CREATE INDEX [IX_Bookmarks_UserId] ON [Bookmarks] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_Categories_Name] ON [Categories] ([Name]);
GO

CREATE UNIQUE INDEX [IX_CommentLikes_CommentId_UserId] ON [CommentLikes] ([CommentId], [UserId]);
GO

CREATE INDEX [IX_CommentLikes_UserId] ON [CommentLikes] ([UserId]);
GO

CREATE INDEX [IX_Comments_BlogId] ON [Comments] ([BlogId]);
GO

CREATE INDEX [IX_Comments_BlogId1] ON [Comments] ([BlogId1]);
GO

CREATE INDEX [IX_Comments_CommentsCommentId] ON [Comments] ([CommentsCommentId]);
GO

CREATE INDEX [IX_Comments_ParentCommentId] ON [Comments] ([ParentCommentId]);
GO

CREATE INDEX [IX_Comments_UserId] ON [Comments] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_DeletedAccounts_UserId] ON [DeletedAccounts] ([UserId]);
GO

CREATE INDEX [IX_Feedbacks_UserId] ON [Feedbacks] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_Follows_FollowerUserId_FollowingUserId] ON [Follows] ([FollowerUserId], [FollowingUserId]);
GO

CREATE INDEX [IX_Follows_FollowingUserId] ON [Follows] ([FollowingUserId]);
GO

CREATE INDEX [IX_Issues_UserId] ON [Issues] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_Likes_BlogId_UserId] ON [Likes] ([BlogId], [UserId]);
GO

CREATE INDEX [IX_Likes_BlogId1] ON [Likes] ([BlogId1]);
GO

CREATE INDEX [IX_Likes_UserId] ON [Likes] ([UserId]);
GO

CREATE INDEX [IX_Notifications_BlogId] ON [Notifications] ([BlogId]);
GO

CREATE INDEX [IX_Notifications_CommentId] ON [Notifications] ([CommentId]);
GO

CREATE INDEX [IX_Notifications_ReceiverUserId] ON [Notifications] ([ReceiverUserId]);
GO

CREATE INDEX [IX_Notifications_SenderUserId] ON [Notifications] ([SenderUserId]);
GO

CREATE INDEX [IX_Payments_PlanId] ON [Payments] ([PlanId]);
GO

CREATE INDEX [IX_Payments_UserId] ON [Payments] ([UserId]);
GO

CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_Reposts_BlogId_UserId] ON [Reposts] ([BlogId], [UserId]);
GO

CREATE INDEX [IX_Reposts_BlogId1] ON [Reposts] ([BlogId1]);
GO

CREATE INDEX [IX_Reposts_UserId] ON [Reposts] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_Tags_Name] ON [Tags] ([Name]);
GO

CREATE INDEX [IX_UserBadges_BadgeId] ON [UserBadges] ([BadgeId]);
GO

CREATE INDEX [IX_UserBadges_BadgesBadgeId] ON [UserBadges] ([BadgesBadgeId]);
GO

CREATE UNIQUE INDEX [IX_UserBadges_UserId_BadgeId] ON [UserBadges] ([UserId], [BadgeId]);
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
GO

CREATE UNIQUE INDEX [IX_Users_UserName] ON [Users] ([UserName]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260813051921_Mig_002', N'8.0.20');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [BlogReports] DROP CONSTRAINT [FK_BlogReports_Blogs_BlogId1];
GO

ALTER TABLE [BlogTags] DROP CONSTRAINT [FK_BlogTags_Tags_TagsTagId];
GO

ALTER TABLE [Bookmarks] DROP CONSTRAINT [FK_Bookmarks_Blogs_BlogId1];
GO

ALTER TABLE [Comments] DROP CONSTRAINT [FK_Comments_Blogs_BlogId1];
GO

ALTER TABLE [Comments] DROP CONSTRAINT [FK_Comments_Comments_CommentsCommentId];
GO

ALTER TABLE [Likes] DROP CONSTRAINT [FK_Likes_Blogs_BlogId1];
GO

ALTER TABLE [Reposts] DROP CONSTRAINT [FK_Reposts_Blogs_BlogId1];
GO

ALTER TABLE [UserBadges] DROP CONSTRAINT [FK_UserBadges_Badges_BadgesBadgeId];
GO

DROP INDEX [IX_UserBadges_BadgesBadgeId] ON [UserBadges];
GO

DROP INDEX [IX_Reposts_BlogId1] ON [Reposts];
GO

DROP INDEX [IX_Likes_BlogId1] ON [Likes];
GO

DROP INDEX [IX_Comments_BlogId1] ON [Comments];
GO

DROP INDEX [IX_Comments_CommentsCommentId] ON [Comments];
GO

DROP INDEX [IX_Bookmarks_BlogId1] ON [Bookmarks];
GO

DROP INDEX [IX_BlogTags_TagsTagId] ON [BlogTags];
GO

DROP INDEX [IX_BlogReports_BlogId1] ON [BlogReports];
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserBadges]') AND [c].[name] = N'BadgesBadgeId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [UserBadges] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [UserBadges] DROP COLUMN [BadgesBadgeId];
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Reposts]') AND [c].[name] = N'BlogId1');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Reposts] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Reposts] DROP COLUMN [BlogId1];
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Likes]') AND [c].[name] = N'BlogId1');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Likes] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [Likes] DROP COLUMN [BlogId1];
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Comments]') AND [c].[name] = N'BlogId1');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Comments] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [Comments] DROP COLUMN [BlogId1];
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Comments]') AND [c].[name] = N'CommentsCommentId');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Comments] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [Comments] DROP COLUMN [CommentsCommentId];
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Bookmarks]') AND [c].[name] = N'BlogId1');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Bookmarks] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [Bookmarks] DROP COLUMN [BlogId1];
GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BlogTags]') AND [c].[name] = N'TagsTagId');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [BlogTags] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [BlogTags] DROP COLUMN [TagsTagId];
GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BlogReports]') AND [c].[name] = N'BlogId1');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [BlogReports] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [BlogReports] DROP COLUMN [BlogId1];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260827054928_Mig_003', N'8.0.20');
GO

COMMIT;
GO

