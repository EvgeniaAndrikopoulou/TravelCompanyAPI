using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using AggregatedAPI_TravelCompanion.Controllers;
using AggregatedAPI_TravelCompanion.Services.AggregationService;
using AggregatedAPI_TravelCompanion.DataTransferObjects.AggregatedDTO;
using AggregatedAPI_TravelCompanion.DataTransferObjects.ExternalApiDTO;
using AggregatedAPI_TravelCompanion.Models.AggregatedModels;
using AggregatedAPI_TravelCompanion.Models.LandmarkModels;

namespace AggregatedAPI_TravelCompanion.Tests;

public class AggregatedDataControllerTests
{
    private readonly Mock<IAggregationService> _mockAggregationService;
    private readonly AggregatedDataController _controller;

    public AggregatedDataControllerTests()
    {
        _mockAggregationService = new Mock<IAggregationService>();
        _controller = new AggregatedDataController(_mockAggregationService.Object);
    }

    [Fact]
    public async Task GetAggregatedDataAsync_ReturnsOk_WhenServiceResponseIsSuccess()
    {
        // Arrange
        var request = new AggregatedDataRequest
        {
            Country = "USA",
            State = "California",
            City = "Los Angeles"
        };

        var serviceResponse = new ServiceResponse<AggregatedData>
        {
            IsSuccess = true,
            Data = new AggregatedData
            {
                Address = "123 Example St, Los Angeles, CA",
                Temperature = 22.5f,
                WeatherDescription = "Clear sky",
                Landmarks = new List<LandmarkInfo>
                    {
                        new LandmarkInfo
                        {
                            Name = "The Getty Center",
                            ShortName = "Museum",
                            FormattedAddress = "1200 Getty Center Dr, Los Angeles, CA"
                        }
                    }
            },
            Message = "Aggregated data retrieved with available results."
        };

        _mockAggregationService
            .Setup(service => service.GetAggregatedDataAsync(It.IsAny<AggregatedDataRequest>()))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await _controller.GetAggregatedDataAsync(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<ServiceResponse<AggregatedData>>(okResult.Value);
        Assert.True(returnValue.IsSuccess);
        Assert.NotNull(returnValue.Data);
        Assert.Equal("123 Example St, Los Angeles, CA", returnValue.Data.Address);
        Assert.Equal(22.5, returnValue.Data.Temperature);
        Assert.Equal("Clear sky", returnValue.Data.WeatherDescription);
    }

    [Fact]
    public async Task GetAggregatedDataAsync_ReturnsBadRequest_WhenServiceResponseIsFailure()
    {
        // Arrange
        var request = new AggregatedDataRequest
        {
            Country = "USA",
            State = "California",
            City = "Los Angeles"
        };

        var serviceResponse = new ServiceResponse<AggregatedData>
        {
            IsSuccess = false,
            Message = "Location info unavailable: API error."
        };

        _mockAggregationService
            .Setup(service => service.GetAggregatedDataAsync(It.IsAny<AggregatedDataRequest>()))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await _controller.GetAggregatedDataAsync(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var returnValue = Assert.IsType<ServiceResponse<AggregatedData>>(badRequestResult.Value);
        Assert.False(returnValue.IsSuccess);
        Assert.Equal("Location info unavailable: API error.", returnValue.Message);
    }
}
