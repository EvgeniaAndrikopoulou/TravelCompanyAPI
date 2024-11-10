namespace AggregatedAPI_TravelCompanion.DataTransferObjects.LocationDTO;

public class LocationRequest
{
    public string Country { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Street { get; set; }
}
