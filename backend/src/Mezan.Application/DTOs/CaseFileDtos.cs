namespace Mezan.Application.DTOs;

public record CaseFileDto(
    string Id,
    string LawyerId,
    string CaseId,
    string Name,
    string Ext,
    long Size,
    string? DataUrl,
    string? Content,
    string? TemplateId,
    long AddedAt
);

public record CreateCaseFileDto(
    string Name,
    string Ext,
    long Size,
    string? DataUrl,
    string? Content,
    string? TemplateId
);
