using Mezan.Application.DTOs;

namespace Mezan.Application.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<LawyerDto?> GetCurrentUserAsync(CancellationToken cancellationToken = default);
}
