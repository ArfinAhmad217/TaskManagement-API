namespace TaskManagement.API.DTOs.Task
{
    public class UpdateTaskRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Priority { get; set; }
        public DateTime? Deadline { get; set; }
        public int? AssignedToUserId { get; set; }
    }
}