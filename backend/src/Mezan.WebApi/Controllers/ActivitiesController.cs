using Mezan.Application.DTOs;
using Mezan.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mezan.WebApi.Controllers;

[Authorize]
public class ActivitiesController : ApiControllerBase
{
    private readonly IActivityService _activityService;

    public ActivitiesController(IActivityService activityService)
    {
        _activityService = activityService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ActivityDto>>> GetActivities([FromQuery] int count = 10, CancellationToken cancellationToken = default)
    {
        var activities = await _activityService.GetRecentActivitiesAsync(count, cancellationToken);
        return Ok(activities);
    }
}
