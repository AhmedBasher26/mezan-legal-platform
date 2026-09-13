using Mezan.Application.DTOs;

namespace Mezan.Application.Services;

public interface ICaseService
{
    Task<List<CaseDto>> GetCasesAsync(string? query = null, string? status = null, CancellationToken cancellationToken = default);
    Task<CaseDto?> GetCaseByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<CaseDto> CreateCaseAsync(CreateCaseDto dto, CancellationToken cancellationToken = default);
    Task<CaseDto> UpdateCaseAsync(string id, UpdateCaseDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteCaseAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> CaseNumberExistsAsync(string caseNumber, string? excludeId = null, CancellationToken cancellationToken = default);
}
