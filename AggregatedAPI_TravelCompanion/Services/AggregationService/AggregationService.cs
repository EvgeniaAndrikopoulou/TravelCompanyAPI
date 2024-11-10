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

        var locationResponse = await _location.GetLocationInfo(new LocationRequest
        {
            Country = request.Country,
            State = request.State,
            City = request.City,
            PostalCode = request.PostalCode,
            Street = request.Street
        });

        if (!locationResponse.IsSuccess)
        {
            serviceResponse.IsSuccess = false;
            serviceResponse.Message = locationResponse.Message;

            return serviceResponse;
        }

        if(locationResponse.Data is null)
        {
            serviceResponse.IsSuccess = false;
            serviceResponse.Message = "Something went wrong with location.";

            return serviceResponse;
        }

        var weatherResponse = await _weather.GetWeatherInfo(new WeatherRequest
        {
            Latitude = locationResponse.Data.Latitude,
            Longitude = locationResponse.Data.Longitude
        });

        if (!weatherResponse.IsSuccess)
        {
            serviceResponse.IsSuccess = false;
            serviceResponse.Message = weatherResponse.Message;

            return serviceResponse;
        }

        if(weatherResponse.Data is null)
        {
            serviceResponse.IsSuccess = false;
            serviceResponse.Message = "Something went wrong with weather.";

            return serviceResponse;
        }

        serviceResponse.Data = new AggregatedData
        {
            Temperature = weatherResponse.Data.Temperature,
            FeeelsLikeTemperature = weatherResponse.Data.FeeelsLikeTemperature,
            WeatherDescription = weatherResponse.Data.Description,
            Landmarks = new List<LandmarkInfo>()
        };

        var landMarkResponse = await _landmark.GetLandmarkInfo(new LandmarkRequest
        {
            Latitude = locationResponse.Data.Latitude,
            Longitude = locationResponse.Data.Longitude,
            Category = request.LandmarkCategory,
            SortBy = request.SortBy,
            SortOrder = request.SortOrder,
            NameFilter = request.LandmarkNameFilter
        });

        if (!landMarkResponse.IsSuccess) 
        {
            serviceResponse.IsSuccess = false;
            serviceResponse.Message = landMarkResponse.Message;

            return serviceResponse;
        }

        if(landMarkResponse.Data is null)
        {
            serviceResponse.IsSuccess = false;
            serviceResponse.Message = "Something went wrong with landmarks.";

            return serviceResponse;
        }

        foreach(var item in landMarkResponse.Data)
        {
            serviceResponse.Data.Landmarks.Add(item);
        }

        serviceResponse.IsSuccess = true;
        serviceResponse.Message = "Aggregated data successfully retrieved.";

        return serviceResponse;
    }
}
