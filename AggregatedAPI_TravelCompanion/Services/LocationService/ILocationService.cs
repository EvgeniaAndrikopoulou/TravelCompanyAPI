using AggregatedAPI_TravelCompanion.DataTransferObjects.ExternalApiDTO;
using AggregatedAPI_TravelCompanion.DataTransferObjects.LocationDTO;
using AggregatedAPI_TravelCompanion.Models.LocationModels;

namespace AggregatedAPI_TravelCompanion.Services.LocationService;

public interface ILocationService
{
    Task<ServiceResponse<LocationInfo>> GetLocationInfo(LocationRequest locationRequest);
}
