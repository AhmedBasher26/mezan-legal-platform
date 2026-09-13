using Mezan.Application.DTOs;

namespace Mezan.Application.Services;

public interface ICaseFileService
{
    Task<List<CaseFileDto>> GetCaseFilesAsync(string caseId, CancellationToken cancellationToken = default);
    Task<CaseFileDto> CreateCaseFileAsync(string caseId, CreateCaseFileDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteCaseFileAsync(string id, CancellationToken cancellationToken = default);
}
