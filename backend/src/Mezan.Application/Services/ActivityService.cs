using Mezan.Application.Common.Interfaces;
using Mezan.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Mezan.Application.Services;

public class ActivityService : IActivityService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ActivityService(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    private string CurrentLawyerId =>
        _currentUserService.LawyerId ?? throw new UnauthorizedAccessException("غير مصرح بالدخول");

    public async Task<List<ActivityDto>> GetRecentActivitiesAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        var acts = await _context.Activities
            .Where(a => a.LawyerId == CurrentLawyerId)
            .OrderByDescending(a => a.At)
            .Take(count)
            .ToListAsync(cancellationToken);

        return acts.Select(a => new ActivityDto(
            a.Id,
            a.LawyerId,
            a.Kind.ToString().ToLowerInvariant(),
            a.Text,
            a.CaseId,
            a.At
        )).ToList();
    }
}
