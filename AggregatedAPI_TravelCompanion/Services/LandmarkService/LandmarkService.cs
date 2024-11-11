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

    public virtual IRestClient CreateRestClient()
    {
        var options = new RestClientOptions("https://api.foursquare.com/v3/places/nearby");
        return new RestClient(options);
    }

    public async Task<ServiceResponse<List<LandmarkInfo>>> GetLandmarkInfo(LandmarkRequest landmarkRequest)
    {
        var serviceResponse = new ServiceResponse<List<LandmarkInfo>>();

        try
        {
            var client = CreateRestClient();
            var request = new RestRequest("");

            request.AddParameter("fields", "fsq_id,name,categories,location,link");
            request.AddParameter("II", $"{landmarkRequest.Latitude}, {landmarkRequest.Longitude}");
            request.AddParameter("limit", "50");
            request.AddParameter("categories", landmarkRequest.Category);

            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", "fsq3uU6ytMSJDfwcCDDE6WG3z5rejVkiF8S1fRUWb92/TdY=");

            var response = await client.ExecuteAsync<LandmarkResponse>(request);

            if (!response.IsSuccessful)
            {
                serviceResponse.IsSuccess = false;
                serviceResponse.Message = $"An error occurred fetching landmark data: {response.StatusCode}";
                return serviceResponse;
            }

            if (response.Data == null || response.Data.results == null || response.Data.results.Length == 0)
            {
                serviceResponse.IsSuccess = false;
                serviceResponse.Message = "No Landmark Data Found";
                return serviceResponse;
            }

            var landmarks = response.Data.results.Select(landmark => new LandmarkInfo
            {
                Name = landmark.name,
                ShortName = landmark.categories.First().short_name,
                FormattedAddress = landmark.location?.formatted_address ?? "Address Unavailable"
            }).ToList();

            if (!string.IsNullOrEmpty(landmarkRequest.NameFilter))
            {
                var filteredLandmarks = FilterLandmarksByName(landmarks, landmarkRequest.NameFilter);

                if (!filteredLandmarks.Any())
                {
                    serviceResponse.Message = $"No landmarks found with the name '{landmarkRequest.NameFilter}'. Returning full landmark list.";
                    serviceResponse.Data = landmarks;
                }
                else
                {
                    serviceResponse.Message = $"Landmarks filtered by name '{landmarkRequest.NameFilter}'.";
                    serviceResponse.Data = filteredLandmarks;
                }
            }

            else if (!string.IsNullOrEmpty(landmarkRequest.SortBy))
            {
                serviceResponse.Data = SortLandmarks(serviceResponse.Data, landmarkRequest.SortBy, landmarkRequest.SortOrder);
            }
            else
            {
                serviceResponse.Data = landmarks;
            }

            serviceResponse.IsSuccess = true;
            return serviceResponse;


        }
        catch (Exception ex)
        {
            serviceResponse.IsSuccess = false;
            serviceResponse.Message = $"An error occurred fetching landmark data: {ex.Message}";

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