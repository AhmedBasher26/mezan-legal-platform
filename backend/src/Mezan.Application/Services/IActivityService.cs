using Mezan.Application.DTOs;

namespace Mezan.Application.Services;

public interface IActivityService
{
    Task<List<ActivityDto>> GetRecentActivitiesAsync(int count = 10, CancellationToken cancellationToken = default);
}
