using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TourPlanner.Bll;
using TourPlanner.Bll.Dtos;
using TourPlanner.Models.MapInformation;

namespace TourPlanner.Api.Controllers;

[ApiController]
[Route("api/ors")]
[Authorize]

public class OpenRouteServiceController : ControllerBase
{
    private readonly IOpenRouteService _openRouteService;
    private readonly ILogger<OpenRouteServiceController> _logger;

    public OpenRouteServiceController(IOpenRouteService openRouteService,ILogger<OpenRouteServiceController> logger)
    {
        _openRouteService = openRouteService;
        _logger = logger;
    }
    
    [HttpGet("coordinates")]
    [ProducesResponseType(typeof(Coordinates),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Coordinates>> GetCoordinates([FromQuery]string location)
    {
        var userId = GetUserId();
        if(userId is null) return Unauthorized();

        try
        {
            var coord = await _openRouteService.GetCoordinates(location);
            return Ok(coord);
        }catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
               Title = "Invalid location",
               Detail = ex.Message,
               Status = StatusCodes.Status400BadRequest 
            });
        }

    }
    [HttpPost("route")]
    [ProducesResponseType(typeof(RouteInformation),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RouteInformation>> GetRouteInformation([FromBody] ORServiceRequestDto request)
    {
        _logger.LogInformation("Receiving request with body: {body}",JsonSerializer.Serialize(request));

        var userId = GetUserId();
        if(userId is null) return Unauthorized();

        try
        {
            var coord = await _openRouteService.GetRouteInformation(request);
            return Ok(coord);
        }catch (ArgumentException ex)
        {
            return NotFound(new ProblemDetails
            {
               Title = "Invalid Route Information",
               Detail = ex.Message,
               Status = StatusCodes.Status404NotFound
            });
        }

    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}