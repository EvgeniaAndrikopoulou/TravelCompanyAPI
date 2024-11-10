using AggregatedAPI_TravelCompanion.DataTransferObjects.ExternalApiDTO;
using AggregatedAPI_TravelCompanion.DataTransferObjects.LandmarkDTO;
using AggregatedAPI_TravelCompanion.Models.LandmarkModels;

namespace AggregatedAPI_TravelCompanion.Services.LandmarkService;

public interface ILandmarkService
{
    Task<ServiceResponse<List<LandmarkInfo>>> GetLandmarkInfo(LandmarkRequest landmarkRequest);
}
