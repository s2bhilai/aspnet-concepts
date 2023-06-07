using Movies.Client.Helpers;
using Movies.Client.Models;
using System.Text.Json;

namespace Movies.Client.Services;

public class LocalStreamsSamples : IIntegrationService
{
    private IHttpClientFactory _httpClientFactory;
    private JsonSerializerOptionsWrapper _jsonSerializationOptionsWrapper;

    public LocalStreamsSamples(IHttpClientFactory httpClientFactory, 
        JsonSerializerOptionsWrapper jsonSerializerOptionsWrapper)
    {
        _httpClientFactory = httpClientFactory;
        _jsonSerializationOptionsWrapper = jsonSerializerOptionsWrapper;
    }

    public async Task RunAsync()
    {
        //await GetPosterWithStreamAsync();
        //await GetPosterWithStreamAndCompletionModeAsync();
        //await PostPosterWithStreamAsync();
        await PostAndReadPosterWithStreamAsync();
    }

    private async Task GetPosterWithStreamAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var request = new HttpRequestMessage(HttpMethod.Get,
            $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
        request.Headers.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        using (var response = await httpClient.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var poster = await JsonSerializer.DeserializeAsync<Poster>(stream,
                _jsonSerializationOptionsWrapper.Options);

            Console.WriteLine(poster?.Name);
        }
    }

    private async Task GetPosterWithStreamAndCompletionModeAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var request = new HttpRequestMessage(HttpMethod.Get,
            $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
        request.Headers.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        using (var response = await httpClient.SendAsync(request,
            HttpCompletionOption.ResponseHeadersRead))
        {
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var poster = await JsonSerializer.DeserializeAsync<Poster>(stream,
                _jsonSerializationOptionsWrapper.Options);

            Console.WriteLine(poster?.Name);
        }
    }

    private async Task PostPosterWithStreamAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var random = new Random();
        var generatedBytes = new byte[5242880];
        random.NextBytes(generatedBytes);

        var posterForCreation = new PosterForCreation()
        {
            Name = "A New Poster",
            Bytes = generatedBytes
        };

        using(var memoryContentStream = new MemoryStream())
        {
            //Convert the entity to memory stream
            await JsonSerializer.SerializeAsync(memoryContentStream, posterForCreation);

            //set the position to start so that data is read from start
            memoryContentStream.Seek(0, SeekOrigin.Begin);

            //In the case of streams, better to use Using
            using(var request = new HttpRequestMessage(HttpMethod.Post,
                "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b/posters"))
            {
                request.Headers.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                //Setting the content for request as stream
                using(var streamContent = new StreamContent(memoryContentStream))
                {
                    streamContent.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    request.Content = streamContent;

                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    var poster = JsonSerializer.Deserialize<Poster>(content, 
                        _jsonSerializationOptionsWrapper.Options);

                    Console.WriteLine($"Poster: {poster.Name}");
                }
            }
        }
    }

    private async Task PostAndReadPosterWithStreamAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var random = new Random();
        var generatedBytes = new byte[5242880];
        random.NextBytes(generatedBytes);

        var posterForCreation = new PosterForCreation()
        {
            Name = "A New Poster",
            Bytes = generatedBytes
        };

        using (var memoryContentStream = new MemoryStream())
        {
            //Convert the entity to memory stream
            await JsonSerializer.SerializeAsync(memoryContentStream, posterForCreation);

            //set the position to start so that data is read from start
            memoryContentStream.Seek(0, SeekOrigin.Begin);

            //In the case of streams, better to use Using
            using (var request = new HttpRequestMessage(HttpMethod.Post,
                "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b/posters"))
            {
                request.Headers.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                //Setting the content for request as stream
                using (var streamContent = new StreamContent(memoryContentStream))
                {
                    streamContent.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    request.Content = streamContent;

                    //start reading content after headers arrive and before entire response arrives
                    var response = await httpClient.SendAsync(request,
                        HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    //Read the content as Stream
                    var stream = await response.Content.ReadAsStreamAsync();
                    var poster = await JsonSerializer.DeserializeAsync<Poster>(stream,
                        _jsonSerializationOptionsWrapper.Options);

                    Console.WriteLine($"Poster Name: {poster.Name}, Poster Id: {poster.Id}");
                }
            }
        }
    }
}
