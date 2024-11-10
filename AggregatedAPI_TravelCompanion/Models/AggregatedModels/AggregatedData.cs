using AggregatedAPI_TravelCompanion.Models.LandmarkModels;

namespace AggregatedAPI_TravelCompanion.Models.AggregatedModels;

public class AggregatedData
{
    public float Temperature { get; set; }
    public float FeeelsLikeTemperature { get; set; }
    public string WeatherDescription { get; set; }
    public List<LandmarkInfo> Landmarks { get; set; }
}
