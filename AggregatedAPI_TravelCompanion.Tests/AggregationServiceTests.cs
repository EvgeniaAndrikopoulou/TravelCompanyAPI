using Moq;
using Xunit;
using System.Threading.Tasks;
using AggregatedAPI_TravelCompanion.Services.AggregationService;
using AggregatedAPI_TravelCompanion.Services.LocationService;
using AggregatedAPI_TravelCompanion.Services.WeatherSevice;
using AggregatedAPI_TravelCompanion.Services.LandmarkService;
using AggregatedAPI_TravelCompanion.DataTransferObjects.AggregatedDTO;
using AggregatedAPI_TravelCompanion.DataTransferObjects.LocationDTO;
using AggregatedAPI_TravelCompanion.DataTransferObjects.WeatherDTO;
using AggregatedAPI_TravelCompanion.DataTransferObjects.LandmarkDTO;
using AggregatedAPI_TravelCompanion.Models.AggregatedModels;
using AggregatedAPI_TravelCompanion.Models.LandmarkModels;
using AggregatedAPI_TravelCompanion.DataTransferObjects.ExternalApiDTO;
using AggregatedAPI_TravelCompanion.Models.LocationModels;
using AggregatedAPI_TravelCompanion.Models.WeatherModels;

namespace AggregatedAPI_TravelCompanion.Tests;

public class AggregationServiceTests
{
    private readonly Mock<ILocationService> _mockLocationService;
    private readonly Mock<IWeatherService> _mockWeatherService;
    private readonly Mock<ILandmarkService> _mockLandmarkService;
    private readonly AggregationService _aggregationService;

    public AggregationServiceTests()
    {
        _mockLocationService = new Mock<ILocationService>();
        _mockWeatherService = new Mock<IWeatherService>();
        _mockLandmarkService = new Mock<ILandmarkService>();
        _aggregationService = new AggregationService(
            _mockLocationService.Object,
            _mockWeatherService.Object,
            _mockLandmarkService.Object
        );
    }

    [Fact]
    public async Task GetAggregatedDataAsync_ReturnsAggregatedData_WhenAllServicesSucceed()
    {
        // Arrange
        var request = new AggregatedDataRequest { Country = "USA", City = "San Francisco" };

        _mockLocationService.Setup(service => service.GetLocationInfo(It.IsAny<LocationRequest>()))
            .ReturnsAsync(new ServiceResponse<LocationInfo>
            {
                IsSuccess = true,
                Data = new LocationInfo { Address = "San Francisco, CA", Latitude = 37.7749f, Longitude = -122.4194f }
            });

        _mockWeatherService.Setup(service => service.GetWeatherInfo(It.IsAny<WeatherRequest>()))
            .ReturnsAsync(new ServiceResponse<WeatherInfo>
            {
                IsSuccess = true,
                Data = new WeatherInfo { Temperature = 20.5f, FeeelsLikeTemperature = 18.5f, Description = "Clear sky" }
            });

        _mockLandmarkService.Setup(service => service.GetLandmarkInfo(It.IsAny<LandmarkRequest>()))
            .ReturnsAsync(new ServiceResponse<List<LandmarkInfo>>
            {
                IsSuccess = true,
                Data = new List<LandmarkInfo> { new LandmarkInfo { Name = "Golden Gate Bridge" } }
            });

        // Act
        var result = await _aggregationService.GetAggregatedDataAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("San Francisco, CA", result.Data.Address);
        Assert.Equal(20.5f, result.Data.Temperature);
        Assert.Single(result.Data.Landmarks);
    }


    [Fact]
    public async Task GetAggregatedDataAsync_ReturnsPartialData_WhenWeatherServiceFails()
    {
        // Arrange
        var request = new AggregatedDataRequest { Country = "USA", City = "San Francisco" };

        _mockLocationService.Setup(service => service.GetLocationInfo(It.IsAny<LocationRequest>()))
            .ReturnsAsync(new ServiceResponse<LocationInfo>
            {
                IsSuccess = true,
                Data = new LocationInfo { Address = "San Francisco, CA", Latitude = 37.7749f, Longitude = -122.4194f }
            });

        _mockWeatherService.Setup(service => service.GetWeatherInfo(It.IsAny<WeatherRequest>()))
            .ReturnsAsync(new ServiceResponse<WeatherInfo> { IsSuccess = false, Message = "Weather API error" });

        _mockLandmarkService.Setup(service => service.GetLandmarkInfo(It.IsAny<LandmarkRequest>()))
            .ReturnsAsync(new ServiceResponse<List<LandmarkInfo>>
            {
                IsSuccess = true,
                Data = new List<LandmarkInfo> { new LandmarkInfo { Name = "Golden Gate Bridge" } }
            });

        // Act
        var result = await _aggregationService.GetAggregatedDataAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains("Weather info unavailable: Weather API error", result.Message);
        Assert.Equal("San Francisco, CA", result.Data.Address);
        Assert.Single(result.Data.Landmarks);
    }

    [Fact]
    public async Task GetAggregatedDataAsync_ReturnsPartialData_WhenLandmarkServiceFails()
    {
        // Arrange
        var request = new AggregatedDataRequest { Country = "USA", City = "San Francisco" };

        _mockLocationService.Setup(service => service.GetLocationInfo(It.IsAny<LocationRequest>()))
            .ReturnsAsync(new ServiceResponse<LocationInfo>
            {
                IsSuccess = true,
                Data = new LocationInfo { Address = "San Francisco, CA", Latitude = 37.7749f, Longitude = -122.4194f }
            });

        _mockWeatherService.Setup(service => service.GetWeatherInfo(It.IsAny<WeatherRequest>()))
            .ReturnsAsync(new ServiceResponse<WeatherInfo>
            {
                IsSuccess = true,
                Data = new WeatherInfo { Temperature = 20.5f, FeeelsLikeTemperature = 18.5f, Description = "Clear sky" }
            });

        _mockLandmarkService.Setup(service => service.GetLandmarkInfo(It.IsAny<LandmarkRequest>()))
            .ReturnsAsync(new ServiceResponse<List<LandmarkInfo>> { IsSuccess = false, Message = "Landmark API error" });

        // Act
        var result = await _aggregationService.GetAggregatedDataAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains("Landmark info unavailable: Landmark API error", result.Message);
        Assert.Equal("San Francisco, CA", result.Data.Address);
        Assert.Equal(20.5f, result.Data.Temperature);
        Assert.Empty(result.Data.Landmarks);
    }

    
}
