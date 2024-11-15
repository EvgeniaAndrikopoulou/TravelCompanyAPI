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

        var uriBuilder = new UriBuilder("https://api.openweathermap.org/data/3.0/onecall/timemachine");

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
            message.Method = HttpMethod.Get;
            
            var apiResponse = await _httpClient.SendAsync(message);

            if (!apiResponse.IsSuccessStatusCode)
            {
                serviceResponse.IsSuccess = false;
                serviceResponse.Message = $"Weather API error: {apiResponse.StatusCode}";
                return serviceResponse;
            }

            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            var apiResponseDto = JsonConvert.DeserializeObject<WeatherResponse>(apiContent);

            if (apiResponseDto?.data == null || !apiResponseDto.data.Any())
            {
                serviceResponse.IsSuccess = false;
                serviceResponse.Message = "No Weather Data Found";
                return serviceResponse;
            }

            var weatherInfo = apiResponseDto.data.First();
            serviceResponse.Data = new WeatherInfo
            {
                Temperature = weatherInfo.temp,
                FeeelsLikeTemperature = weatherInfo.feels_like,
                ShortDescription = weatherInfo.weather[0].main,
                Description = weatherInfo.weather[0].description
            };

            serviceResponse.IsSuccess = true;
            return serviceResponse;
        }
        catch (Exception ex)
        {
            serviceResponse.IsSuccess = false;
            serviceResponse.Message = $"An error occurred fetching weather data: {ex.Message}";

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
