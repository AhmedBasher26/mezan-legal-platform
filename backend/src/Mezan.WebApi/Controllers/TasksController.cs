using Mezan.Application.DTOs;
using Mezan.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mezan.WebApi.Controllers;

[Authorize]
public class TasksController : ApiControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TaskDto>>> GetTasks(
        [FromQuery] string? q,
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] string? date,
        CancellationToken cancellationToken)
    {
        var tasks = await _taskService.GetTasksAsync(q, status, priority, date, cancellationToken);
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskDto>> GetTask(string id, CancellationToken cancellationToken)
    {
        var task = await _taskService.GetTaskByIdAsync(id, cancellationToken);
        if (task == null) return NotFound(new { message = "المهمة غير موجودة" });
        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto dto, CancellationToken cancellationToken)
    {
        var task = await _taskService.CreateTaskAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskDto>> UpdateTask(string id, [FromBody] UpdateTaskDto dto, CancellationToken cancellationToken)
    {
        var task = await _taskService.UpdateTaskAsync(id, dto, cancellationToken);
        return Ok(task);
    }

    [HttpPatch("{id}/toggle")]
    public async Task<ActionResult<TaskDto>> ToggleTask(string id, CancellationToken cancellationToken)
    {
        var task = await _taskService.ToggleTaskAsync(id, cancellationToken);
        return Ok(task);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(string id, CancellationToken cancellationToken)
    {
        var result = await _taskService.DeleteTaskAsync(id, cancellationToken);
        if (!result) return NotFound(new { message = "المهمة غير موجودة" });
        return NoContent();
    }
}
