    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    using TaskManagement.API.DTOs.Task;
    using TaskManagement.API.Services;

    namespace TaskManagement.API.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        [Authorize]
        public class TasksController : ControllerBase
        {
            private readonly TaskService _taskService;

            public TasksController(TaskService taskService)
            {
                _taskService = taskService;
            }

            private int CurrentUserId =>
                int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            private string CurrentUserRole =>
                User.FindFirstValue(ClaimTypes.Role)!;

            [HttpPost]
            [Authorize(Roles = "Admin,Manager")]
            public async Task<IActionResult> CreateTask(CreateTaskRequest request)
            {
                try
                {
                    var result = await _taskService.CreateTask(request, CurrentUserId);
                    return CreatedAtAction(nameof(GetTaskById), new { id = result.Id }, result);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }

            [HttpGet]
            public async Task<IActionResult> GetTasks(
                [FromQuery] string? status,
                [FromQuery] string? priority,
                [FromQuery] DateTime? deadlineFrom,
                [FromQuery] DateTime? deadlineTo)
            {
                var result = await _taskService.GetTasks(
                    CurrentUserId, CurrentUserRole, status, priority, deadlineFrom, deadlineTo);

                return Ok(result);
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetTaskById(int id)
            {
                try
                {
                    var result = await _taskService.GetTaskById(id);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return NotFound(new { message = ex.Message });
                }
            }

            [HttpPut("{id}")]
            [Authorize(Roles = "Admin,Manager")]
            public async Task<IActionResult> UpdateTask(int id, UpdateTaskRequest request)
            {
                try
                {
                    var result = await _taskService.UpdateTask(id, request);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }

            // Admin/Manager/assigned User — sab status update kar sakte
            [HttpPatch("{id}/status")]
            public async Task<IActionResult> UpdateStatus(int id, UpdateTaskStatusRequest request)
            {
                try
                {
                    var result = await _taskService.UpdateStatus(id, request.Status);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }

            [HttpDelete("{id}")]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> DeleteTask(int id)
            {
                try
                {
                    await _taskService.DeleteTask(id);
                    return NoContent();
                }
                catch (Exception ex)
                {
                    return NotFound(new { message = ex.Message });
                }
            }
        }
    }