using AggregatedAPI_TravelCompanion.Configurations;
using AggregatedAPI_TravelCompanion.DataTransferObjects.ExternalApiDTO;
using AggregatedAPI_TravelCompanion.DataTransferObjects.LocationDTO;
using AggregatedAPI_TravelCompanion.Models.LocationModels;
using Newtonsoft.Json;
using System.Web;

namespace AggregatedAPI_TravelCompanion.Services.LocationService;

public class LocationService : ILocationService
{
    private readonly HttpClient _httpClient;

    public LocationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<ServiceResponse<LocationInfo>> GetLocationInfo(LocationRequest locationRequest)
    {
        var serviceResponse = new ServiceResponse<LocationInfo>();

        var uriBuilder = new UriBuilder("https://api.geoapify.com/v1/geocode/search");

        var query = HttpUtility.ParseQueryString(string.Empty);
        query["apiKey"] = UtilityConfig.LocationKeyAPI;
        query["country"] = locationRequest.Country;
        query["state"] = locationRequest.State;
        query["city"] = locationRequest.City;
        query["postcode"] = locationRequest.PostalCode;
        query["street"] = locationRequest.Street;
        
        uriBuilder.Query = query.ToString();

        try
        {

            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");
            
            message.RequestUri = new Uri(uriBuilder.ToString());
            
            HttpResponseMessage apiResponse = new HttpResponseMessage();

            message.Method = HttpMethod.Get;

            apiResponse = await _httpClient.SendAsync(message);

            switch (apiResponse.StatusCode)
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
                    var apiContent = await apiResponse.Content.ReadAsStringAsync();
                    var apiResponseDto = JsonConvert.DeserializeObject<LocationResponse>(apiContent);

                    if(apiResponseDto is null)
                    {
                        serviceResponse.IsSuccess = false;
                        serviceResponse.Message = "No Location Data Found";
                        return serviceResponse;
                    }

                    //var desiredLocation = apiResponseDto.features.FirstOrDefault(l => l.properties.address_line1.Contains(locationRequest.PostalCode.Insert(3, " ")));
                    var desiredLocation = apiResponseDto.features.FirstOrDefault();

                    if (desiredLocation is null)
                    {
                        serviceResponse.IsSuccess = false;
                        serviceResponse.Message = "No Selected Location Found";

                        return serviceResponse;
                    }

                    var locationInfo = new LocationInfo
                    {
                        Longitude = desiredLocation.properties.lon,
                        Latitude = desiredLocation.properties.lat
                    };
                    
                    serviceResponse.IsSuccess = true;
                    serviceResponse.Data = locationInfo;
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
}
