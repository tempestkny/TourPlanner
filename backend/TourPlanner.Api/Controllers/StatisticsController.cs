using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourPlanner.Bll;
using TourPlanner.Bll.Dtos;

namespace TourPlanner.Api.Controllers;

[ApiController]
[Route("api/statistics")]
[Authorize]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        this.statisticsService = statisticsService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(StatisticsResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<StatisticsResponseDto>> Get()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var statistics = await statisticsService.GetStatistics(userId);
        return Ok(statistics);
    }
}
