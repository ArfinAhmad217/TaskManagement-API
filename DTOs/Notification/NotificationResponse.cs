namespace TaskManagement.API.DTOs.Notification
{
    public class NotificationResponse
    {
        public int Id { get; set; }
        public int? TaskItemId { get; set; }
        public string? TaskTitle { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}