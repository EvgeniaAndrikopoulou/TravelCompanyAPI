using AggregatedAPI_TravelCompanion.DataTransferObjects.AggregatedDTO;
using AggregatedAPI_TravelCompanion.DataTransferObjects.ExternalApiDTO;
using AggregatedAPI_TravelCompanion.DataTransferObjects.LandmarkDTO;
using AggregatedAPI_TravelCompanion.DataTransferObjects.LocationDTO;
using AggregatedAPI_TravelCompanion.DataTransferObjects.WeatherDTO;
using AggregatedAPI_TravelCompanion.Models.AggregatedModels;
using AggregatedAPI_TravelCompanion.Models.LandmarkModels;
using AggregatedAPI_TravelCompanion.Services.LandmarkService;
using AggregatedAPI_TravelCompanion.Services.LocationService;
using AggregatedAPI_TravelCompanion.Services.WeatherSevice;

namespace AggregatedAPI_TravelCompanion.Services.AggregationService;

public class AggregationService : IAggregationService
{
    private readonly ILocationService _location;
    private readonly IWeatherService _weather;
    private readonly ILandmarkService _landmark;

    public AggregationService(ILocationService location, IWeatherService weather, ILandmarkService landmark)
    {
        _location = location;
        _weather = weather;
        _landmark = landmark;
    }
    public async Task<ServiceResponse<AggregatedData>> GetAggregatedDataAsync(AggregatedDataRequest request)
    {
        var serviceResponse = new ServiceResponse<AggregatedData>();
        var aggregatedData = new AggregatedData { Landmarks = new List<LandmarkInfo>() };

        var locationResponse = await _location.GetLocationInfo(new LocationRequest
        {
            Country = request.Country,
            State = request.State,
            City = request.City,
            PostalCode = request.PostalCode,
            Street = request.Street
        });

        if (locationResponse.IsSuccess && locationResponse.Data != null)
        {
            aggregatedData.Address = locationResponse.Data.Address;
            var weatherResponse = await _weather.GetWeatherInfo(new WeatherRequest
            {
                Latitude = locationResponse.Data.Latitude,
                Longitude = locationResponse.Data.Longitude
            });

            if(weatherResponse.IsSuccess && weatherResponse.Data != null)
            {
                aggregatedData.Temperature = weatherResponse.Data.Temperature;
                aggregatedData.FeeelsLikeTemperature = weatherResponse.Data.FeeelsLikeTemperature;
                aggregatedData.WeatherDescription = weatherResponse.Data.Description;
            }
            else
            {
                serviceResponse.Message += $"Weather info unavailable: {weatherResponse.Message}. ";
            }

        }
        else
        {
            serviceResponse.Message += $"Location info unavailable: {locationResponse.Message}. ";
        }

        var landMarkResponse = await _landmark.GetLandmarkInfo(new LandmarkRequest
        {
            Latitude = locationResponse?.Data?.Latitude ?? 0,
            Longitude = locationResponse?.Data?.Longitude ?? 0,
            Category = request.LandmarkCategory,
            SortBy = request.SortBy,
            SortOrder = request.SortOrder,
            NameFilter = request.LandmarkNameFilter
        });

        if (landMarkResponse.IsSuccess && landMarkResponse.Data != null)
        {
            aggregatedData.Landmarks = landMarkResponse.Data;
        }
        else
        {
            serviceResponse.Message += $"Landmark info unavailable: {landMarkResponse.Message}. ";
        }

        serviceResponse.Data = aggregatedData;
        serviceResponse.IsSuccess = true;
        serviceResponse.Message += "Aggregated data retrieved with available results.";
        return serviceResponse;
    }
}
