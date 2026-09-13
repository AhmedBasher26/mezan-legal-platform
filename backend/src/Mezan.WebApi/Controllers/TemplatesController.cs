using Mezan.Application.DTOs;
using Mezan.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mezan.WebApi.Controllers;

[Authorize]
public class TemplatesController : ApiControllerBase
{
    private readonly ITemplateService _templateService;

    public TemplatesController(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TemplateDto>>> GetTemplates([FromQuery] string? q, [FromQuery] string? ext, CancellationToken cancellationToken)
    {
        var templates = await _templateService.GetTemplatesAsync(q, ext, cancellationToken);
        return Ok(templates);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TemplateDto>> GetTemplate(string id, CancellationToken cancellationToken)
    {
        var template = await _templateService.GetTemplateByIdAsync(id, cancellationToken);
        if (template == null) return NotFound(new { message = "النموذج غير موجود" });
        return Ok(template);
    }

    [HttpPost]
    public async Task<ActionResult<TemplateDto>> CreateTemplate([FromBody] CreateTemplateDto dto, CancellationToken cancellationToken)
    {
        var template = await _templateService.CreateTemplateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetTemplate), new { id = template.Id }, template);
    }

    [HttpPatch("{id}/rename")]
    public async Task<ActionResult<TemplateDto>> RenameTemplate(string id, [FromBody] RenameTemplateDto dto, CancellationToken cancellationToken)
    {
        var template = await _templateService.RenameTemplateAsync(id, dto, cancellationToken);
        return Ok(template);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTemplate(string id, CancellationToken cancellationToken)
    {
        var result = await _templateService.DeleteTemplateAsync(id, cancellationToken);
        if (!result) return NotFound(new { message = "النموذج غير موجود" });
        return NoContent();
    }
}
