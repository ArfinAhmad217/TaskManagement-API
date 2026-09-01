using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs.Task
{
    public class UpdateTaskStatusRequest
    {
        [Required]
        public string Status { get; set; } = string.Empty; // ToDo/InProgress/Done
    }
}