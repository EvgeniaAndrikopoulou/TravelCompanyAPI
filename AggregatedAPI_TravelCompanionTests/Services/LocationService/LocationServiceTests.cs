using AggregatedAPI_TravelCompanion.DataTransferObjects.LocationDTO;
using Moq;
using Newtonsoft.Json;
using System.Net;
using Moq.Protected;

namespace AggregatedAPI_TravelCompanion.Services.LocationService.Tests;

[TestClass]
public class LocationServiceTests
{
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private HttpClient _httpClient;
    private LocationService _locationService;

    [TestInitialize]
    public void Setup()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _locationService = new LocationService(_httpClient);
    }

    [TestMethod]
    public async Task GetLocationInfo_ShouldReturnLocationInfo_WhenApiResponseIsValid()
    {
        // Arrange
        var locationRequest = new LocationRequest
        {
            Country = "USA",
            State = "California",
            City = "Los Angeles",
            PostalCode = "90001",
            Street = "123 Main St"
        };

        var locationResponse = new LocationResponse
        {
            type = "FeatureCollection",
            features = new[]
            {
                    new Feature
                    {
                        properties = new Properties
                        {
                            lon = -118.2437f,
                            lat = 34.0522f,
                            address_line1 = "123 Main St, 90001"
                        }
                    }
                }
        };

        var jsonResponse = JsonConvert.SerializeObject(locationResponse);

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        // Act
        var result = await _locationService.GetLocationInfo(locationRequest);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual(34.0522f, result.Data.Latitude);
        Assert.AreEqual(-118.2437f, result.Data.Longitude);
    }

    [TestMethod]
    public async Task GetLocationInfo_ShouldReturnFailure_WhenApiResponseIsNotFound()
    {
        // Arrange
        var locationRequest = new LocationRequest
        {
            Country = "InvalidCountry",
            State = "InvalidState",
            City = "InvalidCity",
            PostalCode = "00000",
            Street = "No Street"
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

        // Act
        var result = await _locationService.GetLocationInfo(locationRequest);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Not Found", result.Message);
    }
}