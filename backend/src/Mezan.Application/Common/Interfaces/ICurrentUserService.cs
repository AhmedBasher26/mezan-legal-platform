namespace Mezan.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? LawyerId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}
