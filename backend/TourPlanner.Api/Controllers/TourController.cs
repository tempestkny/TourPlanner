
using Microsoft.AspNetCore.Mvc;
using TourPlanner.Bll;

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
    public string Get()
    {
        return "Called: Get";
    }

    [HttpGet("{id}")]
    public string Get(string id)
    {
        return $"Called: Get \"{id}\" !";   
    }
}