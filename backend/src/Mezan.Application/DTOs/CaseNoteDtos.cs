namespace Mezan.Application.DTOs;

public record CaseNoteDto(
    string Id,
    string LawyerId,
    string CaseId,
    string Text,
    long CreatedAt
);

public record CreateCaseNoteDto(string Text);
