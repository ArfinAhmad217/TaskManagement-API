using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Task
        public int TaskItemId { get; set; }

        public TaskItem TaskItem { get; set; } = null!;

        // User
        public int UserId { get; set; }

        public User User { get; set; } = null!;
    }
}