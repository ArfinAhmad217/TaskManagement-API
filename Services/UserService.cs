using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs.User;
using TaskManagement.API.Models;

namespace TaskManagement.API.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private static readonly string[] AllowedRoles = { "Admin", "Manager", "User" };

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create User (Admin only, via controller check)
        public async Task<UserResponse> CreateUser(CreateUserRequest request)
        {
            if (!AllowedRoles.Contains(request.Role))
            {
                throw new Exception("Invalid role. Allowed: Admin, Manager, User.");
            }

            var existing = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (existing != null)
            {
                throw new Exception("Email already registered.");
            }

            var user = new Models.User
            {
                FullName = request.FullName.Trim(),
                Email = request.Email.Trim().ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return MapToResponse(user);
        }

        // Get All Users (optionally filter by role)
        public async Task<List<UserResponse>> GetAllUsers(string? role = null)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(x => x.Role == role);
            }

            var users = await query
                .OrderBy(x => x.FullName)
                .ToListAsync();

            return users.Select(MapToResponse).ToList();
        }

        // Get User By Id
        public async Task<UserResponse> GetUserById(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return MapToResponse(user);
        }

        private UserResponse MapToResponse(Models.User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }
    }
}