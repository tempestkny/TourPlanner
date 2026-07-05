using Microsoft.Extensions.Logging;
using TourPlanner.Dal;
using TourPlanner.Models;

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

    public TourDto? convertToDto(Tour tour)
    {
        return new TourDto
        {
            id = tour.Id,
            title = tour.Title,
            description = tour.Description,
            from = tour.From,
            to = tour.To,
            transportType = tour.TransportType.ToString(),

            distance = tour.Distance,
            time = tour.Time

        };
    }

    private static TransportType toTransportype(string transportType) => transportType switch
    {
        "Car" => TransportType.Car,
        "Bike" => TransportType.Bike,
        "Hike" => TransportType.Hike,
        _ => TransportType.Car
    };

    /// <summary>
    /// Checks if the TourObject is valid and transfers the object to the Dal
    /// </summary>
    /// <param name="tour"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<string> CreateTour(string userId,TourDto tourDto)
    {
        if(string.IsNullOrWhiteSpace(tourDto.from)) throw new ArgumentException($"{tourDto.title}: A Startpoint is required");
        if(string.IsNullOrWhiteSpace(tourDto.to)) throw new ArgumentException($"{tourDto.title}: An Endpoint is required");
        if(tourDto.transportType == null) throw new ArgumentException($"{tourDto.transportType}: A transportationType is required");
        var tour = new Tour
        {
          Title = tourDto.title,
          Description = tourDto.description,
          From = tourDto.from!,
          To = tourDto.to!,
          TransportType = toTransportype(tourDto.transportType),
          UserId = userId,
        };
        (tour.Time, tour.Distance) = await _openRouteService.GetTimeAndDistance(tourDto.from,tourDto.to, toTransportype(tourDto.transportType));

        await _tourRepository.Create(tour);
        return tour.Id;
    }
    /// <summary>
    /// Removes a Tour from the Database via an Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>true if it could be successfully deleted/ false if the tour to be removed could not be found</returns>
    public async Task<bool> Remove(string id)
    {
        var tour = await _tourRepository.Read(id);
        if(tour is null) return false;
        await _tourRepository.Delete(tour);
        return true;

    }

    public async Task<TourDto?> Get(string id)
    {
        var tour = await _tourRepository.Read(id);
        if (tour is null) return null;
        return convertToDto(tour);
    }

    /// <summary>
    /// Returns a list of tours 
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IEnumerable<TourDto>?> GetTours(string userId,string? query)
    {
        //UsernameExists should be called here if the UserId is invalid
        _logger.LogInformation("Searching tours for user {UserId} with query {Query}", userId, query);
        var tours = await _tourRepository.ReadFromQuery(userId,query);

        return tours?.Select(t => convertToDto(t)).Where(dto => dto is not null).Cast<TourDto>().ToList();
    }

    /// <summary>
    /// Calls the Repository to update a tour
    /// </summary>
    /// <param name="tourId">Id of the Tour to be updated</param>
    /// <param name="newTour"></param>
    /// <returns>true if it was updated/ false if the tourId could not be found</returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<bool> UpdateTour(string tourId, TourDto newTour)
    {
        if(await _tourRepository.Read(tourId) == null) return false;
        
        var tour = new Tour
        {
            Title = newTour.title,
            Description = newTour.description,
            From = newTour.from,
            To = newTour.to,
            TransportType = toTransportype(newTour.transportType),
        };
        await _tourRepository.Update(tourId,tour);

        return true;
    }
}
