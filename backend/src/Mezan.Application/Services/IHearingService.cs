using Mezan.Application.DTOs;

namespace Mezan.Application.Services;

public interface IHearingService
{
    Task<List<HearingDto>> GetHearingsByCaseIdAsync(string caseId, CancellationToken cancellationToken = default);
    Task<List<HearingDto>> GetAgendaHearingsAsync(string? startDate = null, string? endDate = null, CancellationToken cancellationToken = default);
    Task<HearingDto> CreateHearingAsync(string caseId, CreateHearingDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteHearingAsync(string id, CancellationToken cancellationToken = default);
}
