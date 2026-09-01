using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs.User
{
    public class CreateUserRequest
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        // Only "Admin", "Manager", "User" allowed
        [Required]
        public string Role { get; set; } = "User";
    }
}