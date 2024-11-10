namespace AggregatedAPI_TravelCompanion.DataTransferObjects.LandmarkDTO;


public class LandmarkResponse
{
    public Result[] results { get; set; }
}

public class Result
{
    public string fsq_id { get; set; }
    public Category[] categories { get; set; }
    public string link { get; set; }
    public Location location { get; set; }
    public string name { get; set; }
}

public class Location
{
    public string country { get; set; }
    public string cross_street { get; set; }
    public string formatted_address { get; set; }
    public string locality { get; set; }
    public string region { get; set; }
    public string address_extended { get; set; }
    public string postcode { get; set; }
    public string address { get; set; }
}

public class Category
{
    public int id { get; set; }
    public string name { get; set; }
    public string short_name { get; set; }
    public string plural_name { get; set; }
    public Icon icon { get; set; }
}

public class Icon
{
    public string prefix { get; set; }
    public string suffix { get; set; }
}

