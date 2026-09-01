namespace TaskManagement.API.DTOs.Task
{
    public class TaskResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime? Deadline { get; set; }

        public int TeamId { get; set; }
        public string? TeamName { get; set; }

        public int? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }

        public int CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}