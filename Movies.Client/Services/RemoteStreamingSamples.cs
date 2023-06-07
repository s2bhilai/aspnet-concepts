using Movies.Client.Helpers;
using Movies.Client.Models;
using System.Text.Json;

namespace Movies.Client.Services;

public class RemoteStreamingSamples : IIntegrationService
{
    private IHttpClientFactory _httpClientFactory;
    private JsonSerializerOptionsWrapper _jsonSerializerOptionsWrapper;

    public RemoteStreamingSamples(IHttpClientFactory httpClientFactory,
        JsonSerializerOptionsWrapper jsonSerializerOptionsWrapper)
    {
        _httpClientFactory = httpClientFactory;
        _jsonSerializerOptionsWrapper = jsonSerializerOptionsWrapper;
    }

    public async Task RunAsync()
    {
        var client = _httpClientFactory.CreateClient("MoviesAPIClient");

        var request = new HttpRequestMessage(HttpMethod.Get,"api/moviesstream");
        request.Headers.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.SendAsync(request,HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        //regular deserialization
        //var content = await response.Content.ReadAsStringAsync();
        //var movies = JsonSerializer.Deserialize<IEnumerable<Movie>>(
        //    content, _jsonSerializerOptionsWrapper.Options);
        //foreach (var movie in movies)
        //{
        //    Console.WriteLine(movie.Title);
        //}

        //stream the response
        var responseStream = await response.Content.ReadAsStreamAsync();
        var movies = JsonSerializer.DeserializeAsyncEnumerable<Movie>(responseStream,
            _jsonSerializerOptionsWrapper.Options);


        await foreach (var movie in movies)
        {
            Console.WriteLine(movie.Title);
        }


    }
}
