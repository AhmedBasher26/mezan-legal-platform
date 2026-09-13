using Mezan.Application.DTOs;
using Mezan.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mezan.WebApi.Controllers;

[Authorize]
public class CasesController : ApiControllerBase
{
    private readonly ICaseService _caseService;
    private readonly ICaseNoteService _caseNoteService;
    private readonly IHearingService _hearingService;
    private readonly ICaseFileService _caseFileService;

    public CasesController(
        ICaseService caseService,
        ICaseNoteService caseNoteService,
        IHearingService hearingService,
        ICaseFileService caseFileService)
    {
        _caseService = caseService;
        _caseNoteService = caseNoteService;
        _hearingService = hearingService;
        _caseFileService = caseFileService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CaseDto>>> GetCases([FromQuery] string? q, [FromQuery] string? status, CancellationToken cancellationToken)
    {
        var cases = await _caseService.GetCasesAsync(q, status, cancellationToken);
        return Ok(cases);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CaseDto>> GetCase(string id, CancellationToken cancellationToken)
    {
        var cs = await _caseService.GetCaseByIdAsync(id, cancellationToken);
        if (cs == null) return NotFound(new { message = "القضية غير موجودة" });
        return Ok(cs);
    }

    [HttpPost]
    public async Task<ActionResult<CaseDto>> CreateCase([FromBody] CreateCaseDto dto, CancellationToken cancellationToken)
    {
        var cs = await _caseService.CreateCaseAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetCase), new { id = cs.Id }, cs);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CaseDto>> UpdateCase(string id, [FromBody] UpdateCaseDto dto, CancellationToken cancellationToken)
    {
        var cs = await _caseService.UpdateCaseAsync(id, dto, cancellationToken);
        return Ok(cs);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCase(string id, CancellationToken cancellationToken)
    {
        var result = await _caseService.DeleteCaseAsync(id, cancellationToken);
        if (!result) return NotFound(new { message = "القضية غير موجودة" });
        return NoContent();
    }

    // Case Notes
    [HttpGet("{id}/notes")]
    public async Task<ActionResult<List<CaseNoteDto>>> GetNotes(string id, CancellationToken cancellationToken)
    {
        var notes = await _caseNoteService.GetNotesByCaseIdAsync(id, cancellationToken);
        return Ok(notes);
    }

    [HttpPost("{id}/notes")]
    public async Task<ActionResult<CaseNoteDto>> CreateNote(string id, [FromBody] CreateCaseNoteDto dto, CancellationToken cancellationToken)
    {
        var note = await _caseNoteService.CreateNoteAsync(id, dto, cancellationToken);
        return Ok(note);
    }

    // Case Hearings
    [HttpGet("{id}/hearings")]
    public async Task<ActionResult<List<HearingDto>>> GetHearings(string id, CancellationToken cancellationToken)
    {
        var hearings = await _hearingService.GetHearingsByCaseIdAsync(id, cancellationToken);
        return Ok(hearings);
    }

    [HttpPost("{id}/hearings")]
    public async Task<ActionResult<HearingDto>> CreateHearing(string id, [FromBody] CreateHearingDto dto, CancellationToken cancellationToken)
    {
        var hearing = await _hearingService.CreateHearingAsync(id, dto, cancellationToken);
        return Ok(hearing);
    }

    // Case Files
    [HttpGet("{id}/files")]
    public async Task<ActionResult<List<CaseFileDto>>> GetFiles(string id, CancellationToken cancellationToken)
    {
        var files = await _caseFileService.GetCaseFilesAsync(id, cancellationToken);
        return Ok(files);
    }

    [HttpPost("{id}/files")]
    public async Task<ActionResult<CaseFileDto>> CreateFile(string id, [FromBody] CreateCaseFileDto dto, CancellationToken cancellationToken)
    {
        var file = await _caseFileService.CreateCaseFileAsync(id, dto, cancellationToken);
        return Ok(file);
    }
}
