using Mezan.Application.DTOs;

namespace Mezan.Application.Services;

public interface IClientService
{
    Task<List<ClientDto>> GetClientsAsync(string? query = null, CancellationToken cancellationToken = default);
    Task<ClientDto?> GetClientByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<ClientDto> CreateClientAsync(CreateClientDto dto, CancellationToken cancellationToken = default);
    Task<ClientDto> UpdateClientAsync(string id, UpdateClientDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteClientAsync(string id, CancellationToken cancellationToken = default);
    Task<ImportantDateDto> AddImportantDateAsync(string clientId, CreateImportantDateDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteImportantDateAsync(string clientId, string dateId, CancellationToken cancellationToken = default);
}
