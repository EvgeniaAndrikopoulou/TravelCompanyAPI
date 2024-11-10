using AggregatedAPI_TravelCompanion.DataTransferObjects.ExternalApiDTO;
using AggregatedAPI_TravelCompanion.DataTransferObjects.WeatherDTO;
using AggregatedAPI_TravelCompanion.Models.WeatherModels;

namespace AggregatedAPI_TravelCompanion.Services.WeatherSevice;

public interface IWeatherService
{
    Task<ServiceResponse<WeatherInfo>> GetWeatherInfo(WeatherRequest weatherRequest);
}
