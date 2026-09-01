using TaskManagement.API.Models;

namespace TaskManagement.API.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // If Admin already exists, don't create again
            if (context.Users.Any(x => x.Role == "Admin"))
            {
                return;
            }

            var admin = new User
            {
                FullName = "System Admin",
                Email = "admin@taskmanagement.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(admin);

            await context.SaveChangesAsync();
        }
    }
}