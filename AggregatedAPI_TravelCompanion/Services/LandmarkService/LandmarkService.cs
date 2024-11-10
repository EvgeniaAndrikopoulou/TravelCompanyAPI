using AggregatedAPI_TravelCompanion.Configurations;
using AggregatedAPI_TravelCompanion.DataTransferObjects.ExternalApiDTO;
using AggregatedAPI_TravelCompanion.DataTransferObjects.LandmarkDTO;
using AggregatedAPI_TravelCompanion.Models.LandmarkModels;
using RestSharp;
using System.Web;

namespace AggregatedAPI_TravelCompanion.Services.LandmarkService;

public class LandmarkService : ILandmarkService
{
    private readonly HttpClient _httpClient;
    public LandmarkService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<ServiceResponse<List<LandmarkInfo>>> GetLandmarkInfo(LandmarkRequest landmarkRequest)
    {
        var serviceResponse = new ServiceResponse<List<LandmarkInfo>>();

        var uriBuilder = new UriBuilder(UtilityConfig.LocationBaseAPI);

        var query = HttpUtility.ParseQueryString(string.Empty);
        query["fields"] = "fsq_id,name,categories,location,link";
        query["II"] = $"{landmarkRequest.Latitude}, {landmarkRequest.Longitude}";
        query["limit"] = "50";
        query["categories"] = $"{landmarkRequest.Category}";

        uriBuilder.Query = query.ToString();
        
        try
        {

            var options = new RestClientOptions(UtilityConfig.LandmarkBaseAPI);
            var client = new RestClient(options);

            var request = new RestRequest("");
            request.AddParameter("fields", "fsq_id,name,categories,location,link");
            request.AddParameter("II", $"{landmarkRequest.Latitude}, {landmarkRequest.Longitude}");
            request.AddParameter("limit", "50");
            request.AddParameter("categories", landmarkRequest.Category);

            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", UtilityConfig.LandmarkKeyAPI);
            //var response = await client.GetAsync(request);
            var response = await client.ExecuteAsync<LandmarkResponse>(request);


            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.NotFound:
                    return new() { IsSuccess = false, Message = "Not Found" };
                case System.Net.HttpStatusCode.Forbidden:
                    return new() { IsSuccess = false, Message = "Forbidden" };
                case System.Net.HttpStatusCode.Unauthorized:
                    return new() { IsSuccess = false, Message = "Unauthorized" };
                case System.Net.HttpStatusCode.InternalServerError:
                    return new() { IsSuccess = false, Message = "InternalServerError" };
                default:
                    if(response.Data is null)
                    {
                        serviceResponse.IsSuccess = false;
                        serviceResponse.Message = "No Landmark Data Found";
                        return serviceResponse;
                    }
                    
                    serviceResponse.Data = new List<LandmarkInfo>();

                    foreach (var landmark in response.Data.results)
                    {
                        LandmarkInfo item = new LandmarkInfo
                        {
                            Name = landmark.name,
                            ShortName = landmark.categories.First().short_name
                        };
                        if (landmark.location is null)
                        {
                            item.FormattedAddress = "";
                        }

                        item.FormattedAddress = landmark.location.formatted_address;

                        serviceResponse.Data.Add(item);
                    }

                    if (!string.IsNullOrEmpty(landmarkRequest.NameFilter))
                    {
                        serviceResponse.Data = FilterLandmarksByName(serviceResponse.Data, landmarkRequest.NameFilter);
                    }

                    if (!string.IsNullOrEmpty(landmarkRequest.SortBy))
                    { 
                        serviceResponse.Data = SortLandmarks(serviceResponse.Data, landmarkRequest.SortBy, landmarkRequest.SortOrder);                                                                                                                                          
                    }

                    serviceResponse.IsSuccess= true;
                    serviceResponse.Message = "sth";
                    
                    return serviceResponse;

            }
        }
        catch (Exception ex)
        {
            serviceResponse.IsSuccess = false;
            serviceResponse.Message = ex.Message;

            return serviceResponse;
        }
    }

    private List<LandmarkInfo> FilterLandmarksByName(List<LandmarkInfo> landmarks, string nameFilter)
    {
        return landmarks.Where(l => l.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase)).ToList();
    }
    private List<LandmarkInfo> SortLandmarks(List<LandmarkInfo> landmarks, string sortBy, string sortOrder)
    {
        sortBy = string.IsNullOrEmpty(sortBy) ? "name" : sortBy.ToLower();

        sortOrder = string.IsNullOrEmpty(sortOrder) ? "asc" : sortOrder.ToLower();

        if (sortOrder != "asc" && sortOrder != "desc")
        {
            sortOrder = "asc";
        }

        
        switch (sortBy)
        {
            case "name":
                return sortOrder == "desc"
                    ? landmarks.OrderByDescending(l => l.Name).ToList()
                    : landmarks.OrderBy(l => l.Name).ToList();
            default:
                return landmarks; 
        }
    }
}
