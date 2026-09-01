using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "To Do";

        [Required]
        public string Priority { get; set; } = "Medium";

        public DateTime? Deadline { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Team
        public int TeamId { get; set; }

        public Team Team { get; set; } = null!;

        // Assigned User
        public int? AssignedToUserId { get; set; }

        public User? AssignedToUser { get; set; }

        // Created By
        public int CreatedByUserId { get; set; }

        public User CreatedByUser { get; set; } = null!;

        public DateTime? UpdatedAt { get; set; }

        // Comments
        public ICollection<Comment> Comments { get; set; }
            = new List<Comment>();

        // Notifications
        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
    }
}