namespace Mezan.Application.DTOs;

public record TaskDto(
    string Id,
    string LawyerId,
    string Title,
    string? Description,
    string Date,
    string? Time,
    string Priority,
    string? CaseId,
    string? ClientId,
    bool Completed,
    long? CompletedAt,
    long CreatedAt
);

public record CreateTaskDto(
    string Title,
    string? Description,
    string Date,
    string? Time,
    string Priority,
    string? CaseId,
    string? ClientId
);

public record UpdateTaskDto(
    string? Title,
    string? Description,
    string? Date,
    string? Time,
    string? Priority,
    string? CaseId,
    string? ClientId,
    bool? Completed
);
