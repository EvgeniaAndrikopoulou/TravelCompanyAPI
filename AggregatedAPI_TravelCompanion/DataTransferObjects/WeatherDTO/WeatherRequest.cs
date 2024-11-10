namespace AggregatedAPI_TravelCompanion.DataTransferObjects.WeatherDTO;

public class WeatherRequest
{
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    public DateTime RequestedTime { get; set; } = DateTime.Now;
}
