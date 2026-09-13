namespace Mezan.Application.DTOs;

public record CaseDto(
    string Id,
    string LawyerId,
    string CaseNumber,
    string ClientId,
    string Court,
    string CaseType,
    string FiledDate,
    string? NextHearingDate,
    string? NextHearingTime,
    string Status,
    string? Description,
    string? Notes,
    long CreatedAt
);

public record CreateCaseDto(
    string CaseNumber,
    string ClientId,
    string Court,
    string CaseType,
    string FiledDate,
    string? NextHearingDate,
    string? NextHearingTime,
    string Status,
    string? Description,
    string? Notes
);

public record UpdateCaseDto(
    string? CaseNumber,
    string? ClientId,
    string? Court,
    string? CaseType,
    string? FiledDate,
    string? NextHearingDate,
    string? NextHearingTime,
    string? Status,
    string? Description,
    string? Notes
);
