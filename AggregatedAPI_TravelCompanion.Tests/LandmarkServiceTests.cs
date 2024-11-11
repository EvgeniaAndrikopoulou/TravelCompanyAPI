using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using AggregatedAPI_TravelCompanion.Services.LandmarkService;
using AggregatedAPI_TravelCompanion.DataTransferObjects.LandmarkDTO;
using RestSharp;

namespace AggregatedAPI_TravelCompanion.Tests;

public class LandmarkServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly LandmarkService _landmarkService;

    public LandmarkServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _landmarkService = new LandmarkService(_httpClient);
    }

    [Fact]
    public async Task GetLandmarkInfo_ReturnsLandmarks_WhenApiResponseIsValid()
    {
        // Arrange
        var landmarkRequest = new LandmarkRequest
        {
            Latitude = 37.7749f,
            Longitude = -122.4194f,
            Category = "museum",
        };

        var landmarkResponse = new LandmarkResponse
        {
            results = new[]
            {
                new Result
                {
                    name = "The Getty Center",
                    categories = new[]
                    {
                        new Category { short_name = "Museum" }
                    },
                    location = new Location { formatted_address = "1200 Getty Center Dr, Los Angeles, CA" }
                }
            }
        };

        var jsonResponse = JsonConvert.SerializeObject(landmarkResponse);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        // Act
        var result = await _landmarkService.GetLandmarkInfo(landmarkRequest);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetLandmarkInfo_ReturnsError_WhenApiReturnsError()
    {
        // Arrange
        var landmarkRequest = new LandmarkRequest
        {
            Latitude = 37.7749f,
            Longitude = -122.4194f,
            Category = "museum"
        };

        // Mock HttpClient behavior
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Internal Server Error")
        };
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);

        // Create a RestResponse<LandmarkResponse> instance using reflection
        var restResponse = (RestResponse<LandmarkResponse>)Activator.CreateInstance(typeof(RestResponse<LandmarkResponse>), nonPublic: true);

        // Set the properties using reflection
        typeof(RestResponse<LandmarkResponse>).GetProperty("StatusCode")?
            .SetValue(restResponse, HttpStatusCode.InternalServerError);
        typeof(RestResponse<LandmarkResponse>).GetProperty("Content")?
            .SetValue(restResponse, "Internal Server Error");
        typeof(RestResponse<LandmarkResponse>).GetProperty("ResponseStatus")?
            .SetValue(restResponse, ResponseStatus.Completed);

        // Mock RestClient and set up ExecuteAsync to return the configured RestResponse
        var mockRestClient = new Mock<IRestClient>();
        mockRestClient
            .Setup(client => client.ExecuteAsync<LandmarkResponse>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(restResponse);

        // Mock LandmarkService and override CreateRestClient to return the mock RestClient
        var mockLandmarkService = new Mock<LandmarkService>(httpClient) { CallBase = true };
        mockLandmarkService
            .Setup(service => service.CreateRestClient())
            .Returns(mockRestClient.Object);

        // Act
        var result = await mockLandmarkService.Object.GetLandmarkInfo(landmarkRequest);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("An error occurred fetching landmark data: InternalServerError", result.Message);
    }


    [Fact]
    public async Task GetLandmarkInfo_ReturnsError_WhenExceptionOccurs()
    {
        // Arrange
        var landmarkRequest = new LandmarkRequest
        {
            Latitude = 37.7749f,
            Longitude = -122.4194f,
            Category = "museum"
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act
        var result = await _landmarkService.GetLandmarkInfo(landmarkRequest);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("An error occurred fetching landmark data: Network error", result.Message);
    }

    [Fact]
    public async Task GetLandmarkInfo_ReturnsFilteredLandmarks_WhenNameFilterIsApplied()
    {
        // Arrange
        var landmarkRequest = new LandmarkRequest
        {
            Latitude = 37.7749f,
            Longitude = -122.4194f,
            Category = "museum",
            NameFilter = "Getty"
        };

        var landmarkResponse = new LandmarkResponse
        {
            results = new[]
            {
                new Result
                {
                    name = "The Getty Center",
                    categories = new[]
                    {
                        new Category { short_name = "Museum" }
                    },
                    location = new Location { formatted_address = "1200 Getty Center Dr, Los Angeles, CA" }
                },
                new Result
                {
                    name = "Hollywood Walk of Fame",
                    categories = new[]
                    {
                        new Category { short_name = "Tourist" }
                    },
                    location = new Location { formatted_address = "Hollywood Blvd, Los Angeles, CA" }
                }
            }
        };

        var jsonResponse = JsonConvert.SerializeObject(landmarkResponse);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        // Act
        var result = await _landmarkService.GetLandmarkInfo(landmarkRequest);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Data); // Only one landmark should be returned after filtering
        Assert.Equal("The Getty Center", result.Data[0].Name);
    }
}
