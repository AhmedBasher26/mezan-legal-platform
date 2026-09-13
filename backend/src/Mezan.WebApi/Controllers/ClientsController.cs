using Mezan.Application.DTOs;
using Mezan.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mezan.WebApi.Controllers;

[Authorize]
public class ClientsController : ApiControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ClientDto>>> GetClients([FromQuery] string? q, CancellationToken cancellationToken)
    {
        var clients = await _clientService.GetClientsAsync(q, cancellationToken);
        return Ok(clients);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientDto>> GetClient(string id, CancellationToken cancellationToken)
    {
        var client = await _clientService.GetClientByIdAsync(id, cancellationToken);
        if (client == null) return NotFound(new { message = "الموكل غير موجود" });
        return Ok(client);
    }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> CreateClient([FromBody] CreateClientDto dto, CancellationToken cancellationToken)
    {
        var client = await _clientService.CreateClientAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ClientDto>> UpdateClient(string id, [FromBody] UpdateClientDto dto, CancellationToken cancellationToken)
    {
        var client = await _clientService.UpdateClientAsync(id, dto, cancellationToken);
        return Ok(client);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClient(string id, CancellationToken cancellationToken)
    {
        var result = await _clientService.DeleteClientAsync(id, cancellationToken);
        if (!result) return NotFound(new { message = "الموكل غير موجود" });
        return NoContent();
    }

    [HttpPost("{id}/important-dates")]
    public async Task<ActionResult<ImportantDateDto>> AddImportantDate(string id, [FromBody] CreateImportantDateDto dto, CancellationToken cancellationToken)
    {
        var date = await _clientService.AddImportantDateAsync(id, dto, cancellationToken);
        return Ok(date);
    }

    [HttpDelete("{id}/important-dates/{dateId}")]
    public async Task<IActionResult> DeleteImportantDate(string id, string dateId, CancellationToken cancellationToken)
    {
        var result = await _clientService.DeleteImportantDateAsync(id, dateId, cancellationToken);
        if (!result) return NotFound(new { message = "التاريخ غير موجود" });
        return NoContent();
    }
}
