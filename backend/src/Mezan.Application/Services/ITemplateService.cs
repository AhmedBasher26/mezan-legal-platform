using Mezan.Application.DTOs;

namespace Mezan.Application.Services;

public interface ITemplateService
{
    Task<List<TemplateDto>> GetTemplatesAsync(string? query = null, string? ext = null, CancellationToken cancellationToken = default);
    Task<TemplateDto?> GetTemplateByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<TemplateDto> CreateTemplateAsync(CreateTemplateDto dto, CancellationToken cancellationToken = default);
    Task<TemplateDto> RenameTemplateAsync(string id, RenameTemplateDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteTemplateAsync(string id, CancellationToken cancellationToken = default);
}
