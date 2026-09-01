using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relationships
        public ICollection<TeamMember> TeamMemberships { get; set; }
            = new List<TeamMember>();

        public ICollection<TaskItem> AssignedTasks { get; set; }
            = new List<TaskItem>();

        public ICollection<TaskItem> CreatedTasks { get; set; }
            = new List<TaskItem>();

        public ICollection<Comment> Comments { get; set; }
            = new List<Comment>();

        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
    }
}