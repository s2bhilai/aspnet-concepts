using Movies.Client.Helpers;
using Movies.Client.Models;
using System.Text.Json;

namespace Movies.Client.Services;

public class HttpClientFactorySamples : IIntegrationService
{
    private IHttpClientFactory _httpClientFactory;
    private JsonSerializerOptionsWrapper _jsonSerializationWrapper;
    private MoviesAPIClient _moviesAPIClient;

    public HttpClientFactorySamples(IHttpClientFactory httpClientFactory,
        JsonSerializerOptionsWrapper jsonSerializerOptionsWrapper,
        MoviesAPIClient moviesAPIClient)
    {
        _httpClientFactory = httpClientFactory;
        _jsonSerializationWrapper = jsonSerializerOptionsWrapper;
        _moviesAPIClient = moviesAPIClient;
    }


    public async Task RunAsync()
    {
        //await TestDisposeHttpClientAsync();
        //await GetFilmsAsync();
        //await GetMoviesWithTypedHttpClient();
        await GetMoviesWithTypedHttpClientAsync();
    }

    private async Task TestDisposeHttpClientAsync()
    {
        //When HttpClient is disposed off, Connection will be in TIME_WAIT status for 240 seconds, then disposed.
        //Till that 240 secs sockets will not be available, that can lead to socket exhaustion.
        // netstat -abn
        for (int i = 0; i < 10; i++)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://www.google.com");

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Request completed with status code" +
                    $"{response.StatusCode}");
            }
        }
    }

    private async Task GetFilmsAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var request = new HttpRequestMessage(HttpMethod.Get, "api/films");
        request.Headers.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var movies = JsonSerializer.Deserialize<IEnumerable<Movie>>(content,
            _jsonSerializationWrapper.Options);

    }

    private async Task GetMoviesWithTypedHttpClientAsync()
    {
        var movies = await _moviesAPIClient.GetMoviesAsync();

        foreach (var item in movies)
        {
            Console.WriteLine(item.Title);
        }
    }


    //private async Task GetMoviesWithTypedHttpClient()
    //{
    //    var request = new HttpRequestMessage(HttpMethod.Get, "api/movies");
    //    request.Headers.Accept.Add(
    //        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

    //    var response = await _moviesAPIClient.Client.SendAsync(request);
    //    response.EnsureSuccessStatusCode();

    //    var content = await response.Content.ReadAsStringAsync();

    //    var movies = JsonSerializer.Deserialize<IEnumerable<Movie>>(content,
    //        _jsonSerializationWrapper.Options);

    //}
}
