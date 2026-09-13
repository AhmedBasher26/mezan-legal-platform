using Mezan.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mezan.WebApi.Controllers;

[Authorize]
public class CaseFilesController : ApiControllerBase
{
    private readonly ICaseFileService _caseFileService;

    public CaseFilesController(ICaseFileService caseFileService)
    {
        _caseFileService = caseFileService;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCaseFile(string id, CancellationToken cancellationToken)
    {
        var result = await _caseFileService.DeleteCaseFileAsync(id, cancellationToken);
        if (!result) return NotFound(new { message = "المستند غير موجود" });
        return NoContent();
    }
}
