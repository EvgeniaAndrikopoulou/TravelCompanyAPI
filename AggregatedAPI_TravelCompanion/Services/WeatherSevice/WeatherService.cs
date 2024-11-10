﻿using AggregatedAPI_TravelCompanion.Configurations;
using AggregatedAPI_TravelCompanion.DataTransferObjects.ExternalApiDTO;
using AggregatedAPI_TravelCompanion.DataTransferObjects.WeatherDTO;
using AggregatedAPI_TravelCompanion.Models.WeatherModels;
using Newtonsoft.Json;
using System.Web;

namespace AggregatedAPI_TravelCompanion.Services.WeatherSevice;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<ServiceResponse<WeatherInfo>> GetWeatherInfo(WeatherRequest weatherRequest)
    {
        var serviceResponse = new ServiceResponse<WeatherInfo>();

        var uriBuilder = new UriBuilder(UtilityConfig.WeatherBaseAPI);

        var query = HttpUtility.ParseQueryString(string.Empty);
        query["lat"] = weatherRequest.Latitude.ToString();
        query["lon"] = weatherRequest.Longitude.ToString();
        query["dt"] = ToEpochTimestamp(weatherRequest.RequestedTime).ToString();
        query["appid"] = UtilityConfig.WeatherKeyAPI;
        query["units"] = "metric";
        query["lang"] = "en";

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
                    var apiResponseDto = JsonConvert.DeserializeObject<WeatherResponse>(apiContent);

                    if (apiResponseDto is null)
                    {
                        serviceResponse.IsSuccess = false;
                        serviceResponse.Message = "No weather Data Found";
                        return serviceResponse;
                    }

                    var weatherInfo = apiResponseDto.data;
                    if(weatherInfo is null)
                    {
                        serviceResponse.IsSuccess = false;
                        serviceResponse.Message = "No weather Data info Found";
                        return serviceResponse;
                    }

                    serviceResponse.Data = new WeatherInfo
                    {
                        Temperature = weatherInfo[0].temp,
                        FeeelsLikeTemperature = weatherInfo[0].feels_like,
                        ShortDescription = weatherInfo[0].weather[0].main,
                        Description = weatherInfo[0].weather[0].description
                    };
                    

                    serviceResponse.IsSuccess = true;
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

    public long ToEpochTimestamp(DateTime dateTime)
    {

        DateTime utcDateTime = dateTime.ToUniversalTime();

        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        long timestamp = (long)(utcDateTime - epoch).TotalSeconds;

        return timestamp;
    }
}
