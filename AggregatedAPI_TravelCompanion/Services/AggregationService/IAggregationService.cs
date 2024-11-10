using AggregatedAPI_TravelCompanion.DataTransferObjects.AggregatedDTO;
using AggregatedAPI_TravelCompanion.DataTransferObjects.ExternalApiDTO;
using AggregatedAPI_TravelCompanion.Models.AggregatedModels;

namespace AggregatedAPI_TravelCompanion.Services.AggregationService;

public interface IAggregationService
{
    Task<ServiceResponse<AggregatedData>> GetAggregatedDataAsync(AggregatedDataRequest request);
}
