using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs.Comment;
using TaskManagement.API.Models;

namespace TaskManagement.API.Services
{
    public class CommentService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommentResponse> AddComment(int taskItemId, int userId, string content)
        {
            var taskExists = await _context.Tasks.AnyAsync(x => x.Id == taskItemId);
            if (!taskExists)
                throw new Exception("Task not found.");

            var comment = new Comment
            {
                TaskItemId = taskItemId,
                UserId = userId,
                Content = content.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FirstAsync(x => x.Id == userId);

            return new CommentResponse
            {
                Id = comment.Id,
                TaskItemId = comment.TaskItemId,
                UserId = comment.UserId,
                UserName = user.FullName,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt
            };
        }

        public async Task<List<CommentResponse>> GetCommentsForTask(int taskItemId)
        {
            var comments = await _context.Comments
                .Include(x => x.User)
                .Where(x => x.TaskItemId == taskItemId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();

            return comments.Select(x => new CommentResponse
            {
                Id = x.Id,
                TaskItemId = x.TaskItemId,
                UserId = x.UserId,
                UserName = x.User.FullName,
                Content = x.Content,
                CreatedAt = x.CreatedAt
            }).ToList();
        }
    }
}