namespace AggregatedAPI_TravelCompanion.DataTransferObjects.ExternalApiDTO;

public class ServiceResponse<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
}
