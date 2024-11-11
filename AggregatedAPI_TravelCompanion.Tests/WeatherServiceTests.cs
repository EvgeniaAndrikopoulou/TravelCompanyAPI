using Moq;
using Newtonsoft.Json;
using System.Net;
using AggregatedAPI_TravelCompanion.Services.WeatherSevice;
using AggregatedAPI_TravelCompanion.DataTransferObjects.WeatherDTO;
using Moq.Protected;

namespace AggregatedAPI_TravelCompanion.Tests;

public class WeatherServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly WeatherService _weatherService;

    public WeatherServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _weatherService = new WeatherService(_httpClient);
    }

    [Fact]
    public async Task GetWeatherInfo_ReturnsWeatherInfo_WhenApiResponseIsValid()
    {
        var weatherRequest = new WeatherRequest
        {
            Latitude = 37.7749f,
            Longitude = -122.4194f,
            RequestedTime = DateTime.UtcNow.AddHours(-1)
        };

        var weatherResponse = new WeatherResponse
        {
            data = new[]
            {
                new Datum
                {
                    temp = 15.5f,
                    feels_like = 13.0f,
                    weather = new[]
                    {
                        new Weather { main = "Clear", description = "clear sky" }
                    }
                }
            }
        };

        var jsonResponse = JsonConvert.SerializeObject(weatherResponse);
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<System.Threading.CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        var result = await _weatherService.GetWeatherInfo(weatherRequest);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(15.5f, result.Data.Temperature);
        Assert.Equal(13.0f, result.Data.FeeelsLikeTemperature);
        Assert.Equal("Clear", result.Data.ShortDescription);
        Assert.Equal("clear sky", result.Data.Description);
    }

    [Fact]
    public async Task GetWeatherInfo_ReturnsError_WhenApiReturnsError()
    {
        var weatherRequest = new WeatherRequest
        {
            Latitude = 37.7749f,
            Longitude = -122.4194f,
            RequestedTime = DateTime.UtcNow.AddHours(-1)
        };

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Internal Server Error")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<System.Threading.CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        var result = await _weatherService.GetWeatherInfo(weatherRequest);

        Assert.False(result.IsSuccess);
        Assert.Equal("Weather API error: InternalServerError", result.Message);
    }

    [Fact]
    public async Task GetWeatherInfo_ReturnsError_WhenApiReturnsEmptyWeatherData()
    {
        var weatherRequest = new WeatherRequest
        {
            Latitude = 37.7749f,
            Longitude = -122.4194f,
            RequestedTime = DateTime.UtcNow.AddHours(-1)
        };

        var weatherResponse = new WeatherResponse
        {
            data = new Datum[0] 
        };

        var jsonResponse = JsonConvert.SerializeObject(weatherResponse);
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<System.Threading.CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        var result = await _weatherService.GetWeatherInfo(weatherRequest);

        Assert.False(result.IsSuccess);
        Assert.Equal("No Weather Data Found", result.Message);
    }

    [Fact]
    public async Task GetWeatherInfo_ReturnsError_WhenExceptionOccurs()
    {
        var weatherRequest = new WeatherRequest
        {
            Latitude = 37.7749f,
            Longitude = -122.4194f,
            RequestedTime = DateTime.UtcNow.AddHours(-1)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<System.Threading.CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        var result = await _weatherService.GetWeatherInfo(weatherRequest);

        Assert.False(result.IsSuccess);
        Assert.Equal("An error occurred fetching weather data: Network error", result.Message);
    }
}
