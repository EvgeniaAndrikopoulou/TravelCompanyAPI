using AggregatedAPI_TravelCompanion.DataTransferObjects.ExternalApiDTO;
using AggregatedAPI_TravelCompanion.DataTransferObjects.WeatherDTO;
using AggregatedAPI_TravelCompanion.Models.WeatherModels;
using AggregatedAPI_TravelCompanion.Services.WeatherSevice;
using Microsoft.AspNetCore.Mvc;

namespace AggregatedAPI_TravelCompanion.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestWeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public TestWeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpPost]
    public async Task<ServiceResponse<WeatherInfo>> GetWeatherInfo(WeatherRequest weatherRequest)
    {
        return await _weatherService.GetWeatherInfo(weatherRequest);
    }
}
