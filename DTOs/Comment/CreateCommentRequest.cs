using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs.Comment
{
    public class CreateCommentRequest
    {
        [Required]
        public string Content { get; set; } = string.Empty;
    }
}