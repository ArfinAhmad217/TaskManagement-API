using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // User
        public int UserId { get; set; }

        public User User { get; set; } = null!;

        // Task
        public int? TaskItemId { get; set; }

        public TaskItem? TaskItem { get; set; }
    }
}