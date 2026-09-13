namespace Mezan.Application.DTOs;

public record LoginRequest(string Identifier, string Password);

public record RegisterRequest(
    string Name,
    string Email,
    string Phone,
    string Password,
    bool WithDemo = true
);

public record LawyerDto(
    string Id,
    string Name,
    string Email,
    string Phone,
    long CreatedAt
);

public record AuthResponse(
    string Token,
    LawyerDto User
);
