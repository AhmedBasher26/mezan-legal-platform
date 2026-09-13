using Mezan.Application.DTOs;

namespace Mezan.Application.Services;

public interface ITaskService
{
    Task<List<TaskDto>> GetTasksAsync(string? query = null, string? status = null, string? priority = null, string? date = null, CancellationToken cancellationToken = default);
    Task<TaskDto?> GetTaskByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, CancellationToken cancellationToken = default);
    Task<TaskDto> UpdateTaskAsync(string id, UpdateTaskDto dto, CancellationToken cancellationToken = default);
    Task<TaskDto> ToggleTaskAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> DeleteTaskAsync(string id, CancellationToken cancellationToken = default);
}
