using AggregatedAPI_TravelCompanion.DataTransferObjects.ExternalApiDTO;
using AggregatedAPI_TravelCompanion.DataTransferObjects.LocationDTO;
using AggregatedAPI_TravelCompanion.Models.LocationModels;
using AggregatedAPI_TravelCompanion.Services.LocationService;
using Microsoft.AspNetCore.Mvc;

namespace AggregatedAPI_TravelCompanion.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestLocationController : ControllerBase
{
    private readonly ILocationService _locationService;

    public TestLocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpPost]
    public async Task<ServiceResponse<LocationInfo>> GetLocationInfo(LocationRequest locationRequest)
    {
        return await _locationService.GetLocationInfo(locationRequest);
    }
}
