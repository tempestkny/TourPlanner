using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourPlanner.Bll;
using TourPlanner.Bll.Dtos;

namespace TourPlanner.Api.Controllers;

[ApiController]
[Route("api/tourlogs")]
[Authorize]
public class TourLogController : ControllerBase
{
    private readonly ITourLogService tourLogService;

    public TourLogController(ITourLogService tourLogService)
    {
        this.tourLogService = tourLogService;
    }

    [HttpGet("tour/{tourId}")]
    public async Task<ActionResult<IEnumerable<TourLogResponseDto>>> GetByTourId(string tourId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var tourLogs = await tourLogService.GetByTourId(userId, tourId);
        return Ok(tourLogs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TourLogResponseDto>> Get(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var tourLog = await tourLogService.Get(userId, id);
        return tourLog is null ? NotFound() : Ok(tourLog);
    }

    [HttpPost]
    public async Task<ActionResult<TourLogResponseDto>> Create(CreateTourLogDto tourLogDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var createdTourLog = await tourLogService.Create(userId, tourLogDto);
        if (createdTourLog is null) return NotFound();

        return CreatedAtAction(nameof(Get), new { id = createdTourLog.Id }, createdTourLog);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UpdateTourLogDto tourLogDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var updated = await tourLogService.Update(userId, id, tourLogDto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var removed = await tourLogService.Remove(userId, id);
        return removed ? NoContent() : NotFound();
    }
}