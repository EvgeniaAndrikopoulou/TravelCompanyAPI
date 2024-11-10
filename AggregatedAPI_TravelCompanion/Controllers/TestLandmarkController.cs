using AggregatedAPI_TravelCompanion.DataTransferObjects.ExternalApiDTO;
using AggregatedAPI_TravelCompanion.DataTransferObjects.LandmarkDTO;
using AggregatedAPI_TravelCompanion.Models.LandmarkModels;
using AggregatedAPI_TravelCompanion.Services.LandmarkService;
using Microsoft.AspNetCore.Mvc;

namespace AggregatedAPI_TravelCompanion.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestLandmarkController : ControllerBase
{
    private readonly ILandmarkService _landmarkService;

    public TestLandmarkController(ILandmarkService landmarkService)
    {
        _landmarkService = landmarkService;
    }

    [HttpPost]
    public async Task<ServiceResponse<List<LandmarkInfo>>> GetLandmarkInfo(LandmarkRequest landmarkRequest)
    {
        return await _landmarkService.GetLandmarkInfo(landmarkRequest);
    }
}
