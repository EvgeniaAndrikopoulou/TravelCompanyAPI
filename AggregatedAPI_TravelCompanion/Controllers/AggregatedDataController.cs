using AggregatedAPI_TravelCompanion.DataTransferObjects.AggregatedDTO;
using AggregatedAPI_TravelCompanion.Services.AggregationService;
using Microsoft.AspNetCore.Mvc;

namespace AggregatedAPI_TravelCompanion.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AggregatedDataController : ControllerBase
{
    private readonly IAggregationService _aggregationService;

    public AggregatedDataController(IAggregationService aggregationService)
    {
        _aggregationService = aggregationService;
    }

    [HttpPost]
    public async Task<IActionResult> GetAggregatedDataAsync(AggregatedDataRequest request)
    {
        var serviceResponse = await _aggregationService.GetAggregatedDataAsync(request);

        if (!serviceResponse.IsSuccess)
        {
            return BadRequest(serviceResponse);
        }

        return Ok(serviceResponse);
    }
}
