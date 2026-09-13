using Mezan.Application.DTOs;

namespace Mezan.Application.Services;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
}
