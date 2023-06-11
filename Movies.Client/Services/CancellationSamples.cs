using Movies.Client.Helpers;
using Movies.Client.Models;
using System.Text.Json;
using System.Threading;

namespace Movies.Client.Services;

public class CancellationSamples : IIntegrationService
{
    private JsonSerializerOptionsWrapper _jsonSerializationOptionsWrapper;
    private IHttpClientFactory _httpClientFactory;

    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public CancellationSamples(IHttpClientFactory httpClientFactory,
        JsonSerializerOptionsWrapper jsonSerializerOptionsWrapper)
    {
        _jsonSerializationOptionsWrapper = jsonSerializerOptionsWrapper ??
            throw new ArgumentNullException(nameof(jsonSerializerOptionsWrapper));

        _httpClientFactory = httpClientFactory ??
            throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task RunAsync()
    {
        _cancellationTokenSource.CancelAfter(100);
        await GetTrailerAndCancelAsync(_cancellationTokenSource.Token);
    }

    private async Task GetTrailerAndCancelAsync(CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var request = new HttpRequestMessage(HttpMethod.Get,
            $"api/movies/26fcbcc4-b7f7-47fc-9382-740c12246b59/trailers/{Guid.NewGuid()}");

        request.Headers.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.AcceptEncoding.Add(
            new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
       
        //cancellationTokenSource.CancelAfter(100);
        try
        {
            using (var response = await httpClient.SendAsync(request,
            HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                var stream = await response.Content.ReadAsStreamAsync();
                response.EnsureSuccessStatusCode();

                var poster = await JsonSerializer.DeserializeAsync<Trailer>(stream,
                    _jsonSerializationOptionsWrapper.Options);
            }
        }
        catch(OperationCanceledException ocException)
        {
            Console.WriteLine($"An operation was cancelled with message: {ocException.Message}");
        }        
    }

    //To Simulate, reduce the timeout in program class.
    private async Task GetTrailerAndHandleTimeoutAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var request = new HttpRequestMessage(HttpMethod.Get,
            $"api/movies/26fcbcc4-b7f7-47fc-9382-740c12246b59/trailers/{Guid.NewGuid()}");
        request.Headers.Accept.Add(
           new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.AcceptEncoding.Add(
            new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));

        try
        {
            using (var response = await httpClient.SendAsync(request,
            HttpCompletionOption.ResponseHeadersRead))
            {
                var stream = await response.Content.ReadAsStreamAsync();
                response.EnsureSuccessStatusCode();

                var poster = await JsonSerializer.DeserializeAsync<Trailer>(stream,
                    _jsonSerializationOptionsWrapper.Options);
            }
        }
        catch (OperationCanceledException ocException)
        {
            Console.WriteLine($"An operation was cancelled with message: {ocException.Message}");
        }
    }
}
