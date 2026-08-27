using BlogGenerator.DAL;
using BlogGenerator.DomainModels.v1;
using BlogGenerator.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlogGenerator.DAL;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        var guestUser = await db.Users
            .FirstOrDefaultAsync(u => u.Role == UserRole.Guest);

        if (guestUser == null)
        {
            guestUser = new User
            {
                UserName = "system_guest",
                Email = "guest@storybloom.local",
                Role = UserRole.Guest,

                PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                    Guid.NewGuid().ToString()
                ),

                AvailableCredits = 0,
                IsDeleted = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            db.Users.Add(guestUser);

            await db.SaveChangesAsync();
        }
    }
}