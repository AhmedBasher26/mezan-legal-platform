namespace Mezan.Application.DTOs;

public record ActivityDto(
    string Id,
    string LawyerId,
    string Kind,
    string Text,
    string? CaseId,
    long At
);
