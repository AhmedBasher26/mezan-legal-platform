namespace Mezan.Application.DTOs;

public record HearingDto(
    string Id,
    string LawyerId,
    string CaseId,
    string Date,
    string Time,
    string Type,
    string? Notes,
    long CreatedAt
);

public record CreateHearingDto(
    string Date,
    string Time,
    string Type,
    string? Notes
);
