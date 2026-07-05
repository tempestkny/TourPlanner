using System.Text.Json;
using Microsoft.Extensions.Logging;
using TourPlanner.Bll.Dtos;
using TourPlanner.Dal;
using TourPlanner.Models;
using TourPlanner.Models.MapInformation;

namespace TourPlanner.Bll;

public class TourService : ITourService
{
    readonly ITourRepository _tourRepository;
    readonly IOpenRouteService _openRouteService;
    readonly ILogger<TourService> _logger;
    public TourService(ITourRepository tourRepository,IOpenRouteService openRouteService,ILogger<TourService> logger)
    {
        _tourRepository = tourRepository;
        _openRouteService = openRouteService;
        _logger = logger;
    }

    public static TourResponseDto ConvertToResponseDto(Tour tour)
    {
        return new TourResponseDto
        {
            Id = tour.Id,
            Title = tour.Title,
            Description = tour.Description,
            From = tour.From,
            To = tour.To,
            TransportType = tour.TransportType,
            route = JsonSerializer.Deserialize<RouteInformation>(tour.RouteInfo)
        };
    }

    /// <summary>
    /// Checks if the TourObject is valid and transfers the object to the Dal
    /// </summary>
    /// <param name="tour"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<string> CreateTour(string userId, CreateTourDto createTourDto)
    {
        if (string.IsNullOrWhiteSpace(createTourDto.From)) throw new ArgumentException($"{createTourDto.Title}: A Startpoint is required");
        if (string.IsNullOrWhiteSpace(createTourDto.To)) throw new ArgumentException($"{createTourDto.Title}: An Endpoint is required");
        var tour = new Tour
        {
            Title = createTourDto.Title,
            Description = createTourDto.Description,
            From = createTourDto.From!,
            To = createTourDto.To!,
            TransportType = createTourDto.TransportType,
            UserId = userId,
            RouteInfo = JsonSerializer.Serialize(createTourDto.Route)
        };

        await _tourRepository.Create(tour);
        return tour.Id;
    }
    /// <summary>
    /// Removes a Tour from the Database via an Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>true if it could be successfully deleted/ false if the tour to be removed could not be found</returns>
    public async Task<bool> RemoveTour(string id)
    {
        var tour = await _tourRepository.Read(id);
        if (tour is null) return false;
        await _tourRepository.Delete(tour);
        return true;

    }

    public async Task<TourResponseDto?> GetTour(string id)
    {
        var tour = await _tourRepository.Read(id);
        if (tour is null) return null;
        return ConvertToResponseDto(tour);
    }

    public async Task<RouteInformationResponseDto?> GetRouteInformation(string TourId)
    {
        var tour = await _tourRepository.Read(TourId);
        if (tour is null) return null;
        return new RouteInformationResponseDto
        {
            Route = JsonSerializer.Deserialize<RouteInformation>(tour.RouteInfo)
        };
    }

    /// <summary>
    /// Returns a list of tours 
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IEnumerable<TourResponseDto>?> GetTours(string userId, string? query)
    {
        //UsernameExists should be called here if the UserId is invalid
        _logger.LogInformation($"Searching for Tours. query: {query}");
        var tours = await _tourRepository.ReadFromQuery(userId,query);

        return tours?.Select(t => ConvertToResponseDto(t)).Where(dto => dto is not null).Cast<TourResponseDto>().ToList();
    }

    /// <summary>
    /// Calls the Repository to update a tour
    /// </summary>
    /// <param name="tourId">Id of the Tour to be updated</param>
    /// <param name="newTour"></param>
    /// <returns>true if it was updated/ false if the tourId could not be found</returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<bool> UpdateTour(string tourId, UpdateTourDto newTour)
    {
        if (await _tourRepository.Read(tourId) == null) return false;

        var tour = new Tour
        {
            Title = newTour.Title,
            Description = newTour.Description,
            From = newTour.From,
            To = newTour.To,
            TransportType = (TransportType)newTour.TransportType!,
            RouteInfo = JsonSerializer.Serialize(newTour.Route)
        };
        await _tourRepository.Update(tourId, tour);

        return true;
    }
}
