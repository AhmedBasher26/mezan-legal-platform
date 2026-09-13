namespace Mezan.Application.DTOs;

public record TemplateDto(
    string Id,
    string LawyerId,
    string Name,
    string Ext,
    long Size,
    string Kind,
    string? Content,
    string? DataUrl,
    long AddedAt
);

public record CreateTemplateDto(
    string Name,
    string Ext,
    long Size,
    string Kind,
    string? Content,
    string? DataUrl
);

public record RenameTemplateDto(string Name);
