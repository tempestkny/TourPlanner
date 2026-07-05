using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourPlanner.Bll;
using TourPlanner.Bll.Dtos;

namespace TourPlanner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TourController : ControllerBase
{
    private readonly ITourService _tourService;

    public TourController(ITourService tourService)
    {
        _tourService = tourService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TourResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<TourResponseDto>>> GetAll([FromQuery] string? query)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var tours = await _tourService.GetTours(userId!, query);
        return tours is null ? NotFound() : Ok(tours);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TourResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TourResponseDto>> Get(string id)
    {
        var tour = await _tourService.GetTour(id);
        return tour is null ? NotFound() : Ok(tour);
    }

    [HttpGet("{id}/route")]
    [ProducesResponseType(typeof(TourResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RouteInformationResponseDto>> GetRoute(string TourId)
    {
        var route = await _tourService.GetRouteInformation(TourId);
        return route is null ? NotFound() : Ok(route);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<string>> Create([FromBody] CreateTourDto tour)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId)) return Unauthorized(new ProblemDetails
        {
            Title = "Id could not be extracted from Token",
            Detail = $"Token: {userId}",
            Status = StatusCodes.Status500InternalServerError
        });

        try
        {
            var id = await _tourService.CreateTour(userId!, tour);
            return CreatedAtAction(nameof(Get), new { id }, new { id });
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateTourDto tour)
    {
        var updated = await _tourService.UpdateTour(id, tour);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(string id)
    {
        var removed = await _tourService.RemoveTour(id);
        return removed ? NoContent() : NotFound();
    }
}