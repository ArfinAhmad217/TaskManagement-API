using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.API.DTOs.Comment;
using TaskManagement.API.Services;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/comments")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly CommentService _commentService;

        public CommentsController(CommentService commentService)
        {
            _commentService = commentService;
        }

        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // POST: api/comments/task/5
        [HttpPost("task/{taskId}")]
        public async Task<IActionResult> AddComment(int taskId, CreateCommentRequest request)
        {
            try
            {
                var result = await _commentService.AddComment(taskId, CurrentUserId, request.Content);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/comments/task/5
        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetComments(int taskId)
        {
            var result = await _commentService.GetCommentsForTask(taskId);
            return Ok(result);
        }
    }
}