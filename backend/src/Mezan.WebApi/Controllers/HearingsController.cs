using Mezan.Application.DTOs;
using Mezan.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mezan.WebApi.Controllers;

[Authorize]
public class HearingsController : ApiControllerBase
{
    private readonly IHearingService _hearingService;

    public HearingsController(IHearingService hearingService)
    {
        _hearingService = hearingService;
    }

    [HttpGet("agenda")]
    public async Task<ActionResult<List<HearingDto>>> GetAgenda([FromQuery] string? startDate, [FromQuery] string? endDate, CancellationToken cancellationToken)
    {
        var hearings = await _hearingService.GetAgendaHearingsAsync(startDate, endDate, cancellationToken);
        return Ok(hearings);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHearing(string id, CancellationToken cancellationToken)
    {
        var result = await _hearingService.DeleteHearingAsync(id, cancellationToken);
        if (!result) return NotFound(new { message = "الجلسة غير موجودة" });
        return NoContent();
    }
}
