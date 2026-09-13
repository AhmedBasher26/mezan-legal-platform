using Mezan.Application.Common.Interfaces;
using Mezan.Application.DTOs;
using Mezan.Domain.Entities;
using Mezan.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Mezan.Application.Services;

public class TaskService : ITaskService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public TaskService(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    private string CurrentLawyerId =>
        _currentUserService.LawyerId ?? throw new UnauthorizedAccessException("غير مصرح بالدخول");

    public async Task<List<TaskDto>> GetTasksAsync(string? query = null, string? status = null, string? priority = null, string? date = null, CancellationToken cancellationToken = default)
    {
        var q = _context.Tasks
            .Include(t => t.Case)
            .Include(t => t.Client)
            .Where(t => t.LawyerId == CurrentLawyerId);

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (status == "open") q = q.Where(t => !t.Completed);
            else if (status == "done") q = q.Where(t => t.Completed);
        }

        if (!string.IsNullOrWhiteSpace(priority) && priority != "all" && Enum.TryParse<Priority>(priority, true, out var p))
        {
            q = q.Where(t => t.Priority == p);
        }

        if (!string.IsNullOrWhiteSpace(date))
        {
            q = q.Where(t => t.Date == date);
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            var s = query.Trim().ToLower();
            q = q.Where(t =>
                t.Title.ToLower().Contains(s) ||
                (t.Case != null && t.Case.CaseNumber.ToLower().Contains(s)) ||
                (t.Client != null && t.Client.Name.ToLower().Contains(s)));
        }

        var tasks = await q.OrderBy(t => t.Date).ThenBy(t => t.Time).ToListAsync(cancellationToken);
        return tasks.Select(MapToDto).ToList();
    }

    public async Task<TaskDto?> GetTaskByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.LawyerId == CurrentLawyerId, cancellationToken);

        return task == null ? null : MapToDto(task);
    }

    public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, CancellationToken cancellationToken = default)
    {
        Enum.TryParse<Priority>(dto.Priority, true, out var prio);

        var task = new TaskItem
        {
            LawyerId = CurrentLawyerId,
            Title = dto.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            Date = dto.Date.Trim(),
            Time = string.IsNullOrWhiteSpace(dto.Time) ? null : dto.Time.Trim(),
            Priority = prio,
            CaseId = string.IsNullOrWhiteSpace(dto.CaseId) ? null : dto.CaseId,
            ClientId = string.IsNullOrWhiteSpace(dto.ClientId) ? null : dto.ClientId,
            Completed = false
        };

        _context.Tasks.Add(task);

        _context.Activities.Add(new Activity
        {
            LawyerId = CurrentLawyerId,
            Kind = ActivityKind.Task,
            Text = $"تمت إضافة مهمة جديدة: {task.Title}",
            CaseId = task.CaseId
        });

        await _context.SaveChangesAsync(cancellationToken);
        return MapToDto(task);
    }

    public async Task<TaskDto> UpdateTaskAsync(string id, UpdateTaskDto dto, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.LawyerId == CurrentLawyerId, cancellationToken)
            ?? throw new KeyNotFoundException("المهمة غير موجودة");

        if (dto.Title != null) task.Title = dto.Title.Trim();
        if (dto.Description != null) task.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
        if (dto.Date != null) task.Date = dto.Date.Trim();
        if (dto.Time != null) task.Time = string.IsNullOrWhiteSpace(dto.Time) ? null : dto.Time.Trim();

        if (dto.Priority != null && Enum.TryParse<Priority>(dto.Priority, true, out var prio))
        {
            task.Priority = prio;
        }

        if (dto.CaseId != null) task.CaseId = string.IsNullOrWhiteSpace(dto.CaseId) ? null : dto.CaseId;
        if (dto.ClientId != null) task.ClientId = string.IsNullOrWhiteSpace(dto.ClientId) ? null : dto.ClientId;

        if (dto.Completed.HasValue)
        {
            task.Completed = dto.Completed.Value;
            task.CompletedAt = task.Completed ? DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() : null;
        }

        task.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        await _context.SaveChangesAsync(cancellationToken);
        return MapToDto(task);
    }

    public async Task<TaskDto> ToggleTaskAsync(string id, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.LawyerId == CurrentLawyerId, cancellationToken)
            ?? throw new KeyNotFoundException("المهمة غير موجودة");

        task.Completed = !task.Completed;
        task.CompletedAt = task.Completed ? DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() : null;
        task.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        if (task.Completed)
        {
            _context.Activities.Add(new Activity
            {
                LawyerId = CurrentLawyerId,
                Kind = ActivityKind.Task,
                Text = $"تم إكمال مهمة: {task.Title}",
                CaseId = task.CaseId
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        return MapToDto(task);
    }

    public async Task<bool> DeleteTaskAsync(string id, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.LawyerId == CurrentLawyerId, cancellationToken);

        if (task == null) return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static TaskDto MapToDto(TaskItem t) =>
        new(
            t.Id,
            t.LawyerId,
            t.Title,
            t.Description,
            t.Date,
            t.Time,
            t.Priority.ToString().ToLowerInvariant(),
            t.CaseId,
            t.ClientId,
            t.Completed,
            t.CompletedAt,
            t.CreatedAt
        );
}
