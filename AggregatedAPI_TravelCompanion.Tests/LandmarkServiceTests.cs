using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using AggregatedAPI_TravelCompanion.Services.LandmarkService;
using AggregatedAPI_TravelCompanion.DataTransferObjects.LandmarkDTO;

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

        var result = await _landmarkService.GetLandmarkInfo(landmarkRequest);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

}