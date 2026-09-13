using System.Security.Claims;
using Mezan.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Mezan.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? LawyerId =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? Email =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
