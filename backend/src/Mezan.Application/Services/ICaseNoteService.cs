using Mezan.Application.DTOs;

namespace Mezan.Application.Services;

public interface ICaseNoteService
{
    Task<List<CaseNoteDto>> GetNotesByCaseIdAsync(string caseId, CancellationToken cancellationToken = default);
    Task<CaseNoteDto> CreateNoteAsync(string caseId, CreateCaseNoteDto dto, CancellationToken cancellationToken = default);
}
