using Mezan.Application.DTOs;
using Mezan.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mezan.WebApi.Controllers;

[Authorize]
public class DashboardController : ApiControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary(CancellationToken cancellationToken)
    {
        var summary = await _dashboardService.GetSummaryAsync(cancellationToken);
        return Ok(summary);
    }
}
