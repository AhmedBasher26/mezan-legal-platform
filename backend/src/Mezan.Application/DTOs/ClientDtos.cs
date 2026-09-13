namespace Mezan.Application.DTOs;

public record ImportantDateDto(
    string Id,
    string Label,
    string Date
);

public record CreateImportantDateDto(
    string Label,
    string Date
);

public record ClientDto(
    string Id,
    string LawyerId,
    string Name,
    string Phone,
    string Email,
    string? NationalId,
    string? Address,
    string? Notes,
    List<ImportantDateDto> ImportantDates,
    long CreatedAt
);

public record CreateClientDto(
    string Name,
    string Phone,
    string Email,
    string? NationalId,
    string? Address,
    string? Notes
);

public record UpdateClientDto(
    string? Name,
    string? Phone,
    string? Email,
    string? NationalId,
    string? Address,
    string? Notes
);
