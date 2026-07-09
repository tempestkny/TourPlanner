using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourPlanner.Bll;
using TourPlanner.Bll.Dtos;

namespace TourPlanner.Api.Controllers;

[ApiController]
[Route("api/import-export")]
[Authorize] // only Authenticated users can access these endpoints
public class ImportExportController : ControllerBase
{ // DI
    private readonly IImportExportService importExportService;

    public ImportExportController(IImportExportService importExportService)
    {
        this.importExportService = importExportService;
    }

    [HttpGet("export")]
    [ProducesResponseType(typeof(ExportTourDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ExportTourDataDto>> Export()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized(); // check if authenticated 

        var exportData = await importExportService.Export(userId); // BLL Service
        return Ok(exportData);
    }

    [HttpPost("import")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Import(ImportTourDataDto importData)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        try
        {
            var importedTours = await importExportService.Import(userId, importData);
            return Ok(new { importedTours });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid import payload",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    private string? GetUserId()
    { // Get userID from claims
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
