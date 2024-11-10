using System;
namespace AggregatedAPI_TravelCompanionTests.Utilities;

public class MockHttpMessageHandler : HttpMessageHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // This method will be overridden by the Moq setup.
        return await Task.FromResult(new HttpResponseMessage());
    }
}
