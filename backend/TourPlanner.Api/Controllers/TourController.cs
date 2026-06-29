using Microsoft.AspNetCore.Mvc;
using TourPlanner.Bll;
using TourPlanner.Models;

namespace TourPlanner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TourController : ControllerBase
{
    private readonly ITourService _tourService;

    public TourController(ITourService tourService)
    {
        _tourService = tourService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TourDto>>> GetAll([FromQuery] string userId, [FromQuery] string? query = null)
    {
        var tours = await _tourService.GetTours(userId, query);
        return tours is null ? NotFound() : Ok(tours);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TourDto>> Get(string id)
    {
        var tour = await _tourService.Get(id);
        return tour is null ? NotFound() : Ok(tour);
    }

    [HttpPost("{userId}")]
    public async Task<ActionResult<string>> Create(string userId, [FromBody] TourDto tour)
    {
        try
        {
            var id = await _tourService.CreateTour(userId, tour);
            return CreatedAtAction(nameof(Get), new { id }, id);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid tour payload",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] TourDto tour)
    {
        var updated = await _tourService.UpdateTour(id, tour);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var removed = await _tourService.Remove(id);
        return removed ? NoContent() : NotFound();
    }
}