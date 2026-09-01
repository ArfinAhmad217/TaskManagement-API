using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models
{
    public class Team
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Manager responsible for this team
        public int? ManagerId { get; set; }

        public User? Manager { get; set; }

        // Team Members
        public ICollection<TeamMember> Members { get; set; }
            = new List<TeamMember>();

        // Team Tasks
        public ICollection<TaskItem> Tasks { get; set; }
            = new List<TaskItem>();
    }
}