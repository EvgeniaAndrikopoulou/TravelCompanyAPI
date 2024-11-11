using Moq;
using Newtonsoft.Json;
using System.Net;
using AggregatedAPI_TravelCompanion.Services.LocationService;
using AggregatedAPI_TravelCompanion.DataTransferObjects.LocationDTO;
using Moq.Protected;

namespace AggregatedAPI_TravelCompanion.Tests;

public class LocationServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly LocationService _locationService;

    public LocationServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);

        _locationService = new LocationService(_httpClient);
    }

    [Fact]
    public async Task GetLocationInfo_ReturnsLocationInfo_WhenApiResponseIsValid()
    {
        var locationRequest = new LocationRequest
        {
            Country = "USA",
            State = "CA",
            City = "San Francisco",
            PostalCode = "94105",
            Street = "Market Street"
        };

        var locationResponse = new LocationResponse
        {
            features = new[]
            {
                new Feature
                {
                    properties = new Properties
                    {
                        lon = -122.4194f,
                        lat = 37.7749f,
                        address_line2 = "Some Address"
                    }
                }
            }
        };

        var jsonResponse = JsonConvert.SerializeObject(locationResponse);

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

        var result = await _locationService.GetLocationInfo(locationRequest);

        Assert.True(result.IsSuccess);  
        Assert.NotNull(result.Data);  
        Assert.Equal(-122.4194f, result.Data.Longitude);  
        Assert.Equal(37.7749f, result.Data.Latitude);  
        Assert.Equal("Some Address", result.Data.Address);
    }

    [Fact]
    public async Task GetLocationInfo_ReturnsError_WhenApiReturnsError()
    {
        var locationRequest = new LocationRequest
        {
            Country = "USA",
            State = "CA",
            City = "San Francisco",
            PostalCode = "94105",
            Street = "Market Street"
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

        var result = await _locationService.GetLocationInfo(locationRequest);

        Assert.False(result.IsSuccess);
        Assert.Equal("Location API error: InternalServerError", result.Message);
    }

    [Fact]
    public async Task GetLocationInfo_ReturnsError_WhenApiReturnsEmptyLocationData()
    {
        var locationRequest = new LocationRequest
        {
            Country = "USA",
            State = "CA",
            City = "San Francisco",
            PostalCode = "94105",
            Street = "Market Street"
        };

        var locationResponse = new LocationResponse
        {
            features = new Feature[0]
        };

        var jsonResponse = JsonConvert.SerializeObject(locationResponse);

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

        var result = await _locationService.GetLocationInfo(locationRequest);

        
        Assert.False(result.IsSuccess);  
        Assert.Equal("No Location Data Found", result.Message); 
    }

    [Fact]
    public async Task GetLocationInfo_ReturnsError_WhenExceptionOccurs()
    {
        var locationRequest = new LocationRequest
        {
            Country = "USA",
            State = "CA",
            City = "San Francisco",
            PostalCode = "94105",
            Street = "Market Street"
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<System.Threading.CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        var result = await _locationService.GetLocationInfo(locationRequest);

        Assert.False(result.IsSuccess);
        Assert.Equal("An error occurred fetching location data: Network error", result.Message);
    }


}
