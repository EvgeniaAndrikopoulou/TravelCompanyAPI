namespace AggregatedAPI_TravelCompanion.DataTransferObjects.AggregatedDTO;

public class AggregatedDataRequest
{
    public string Country { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Street { get; set; }
    public DateTime RequestedTime { get; set; } = DateTime.Now;
    public string LandmarkCategory { get; set; }
    public string SortBy { get; set; }
    public string SortOrder { get; set; } = "asc";
    public string LandmarkNameFilter { get; set; }
}
