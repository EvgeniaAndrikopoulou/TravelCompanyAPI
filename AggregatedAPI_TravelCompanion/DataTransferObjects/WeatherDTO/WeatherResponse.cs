namespace AggregatedAPI_TravelCompanion.DataTransferObjects.WeatherDTO;

public class WeatherResponse
{
    public float lat { get; set; }
    public float lon { get; set; }
    public string timezone { get; set; }
    public int timezone_offset { get; set; }
    public Datum[] data { get; set; }
}

public class Datum
{
    public float dt { get; set; }
    public float sunrise { get; set; }
    public float sunset { get; set; }
    public float temp { get; set; }
    public float feels_like { get; set; }
    public float pressure { get; set; }
    public float humidity { get; set; }
    public float dew_point { get; set; }
    public float uvi { get; set; }
    public float clouds { get; set; }
    public float visibility { get; set; }
    public float wind_speed { get; set; }
    public float wind_deg { get; set; }
    public float wind_gust { get; set; }
    public Weather[] weather { get; set; }
}

public class Weather
{
    public int id { get; set; }
    public string main { get; set; }
    public string description { get; set; }
    public string icon { get; set; }
}
