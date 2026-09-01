using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs.Task
{
    public class CreateTaskRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public string Priority { get; set; } = "Medium";

        public DateTime? Deadline { get; set; }

        [Required]
        public int TeamId { get; set; }

        public int? AssignedToUserId { get; set; }
    }
}