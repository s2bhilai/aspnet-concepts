

using Movies.Client.Helpers;
using Movies.Client.Models;
using System.Text.Json;
using System.Xml.Serialization;

namespace Movies.Client.Services;

public class CRUDSamples : IIntegrationService
{
    private IHttpClientFactory _httpClientFactory;
    private JsonSerializerOptionsWrapper _jsonSerializerOptionsWrapper;

    public CRUDSamples(IHttpClientFactory httpClientFactory,
        JsonSerializerOptionsWrapper jsonSerializerOptionsWrapper)
    {
        _httpClientFactory = httpClientFactory ??
            throw new ArgumentNullException(nameof(httpClientFactory));

        _jsonSerializerOptionsWrapper =  jsonSerializerOptionsWrapper ?? 
            throw new ArgumentNullException(nameof(jsonSerializerOptionsWrapper));
    }

    public async Task RunAsync()
    {
        await GetResourceAsync();
        //await GetResourceThroughHttpRequestMessageAsync();
        //await CreateResourceAsync();
        //await UpdateResourceAsync();
        //await DeleteResourceAsync();
    }

    public async Task GetResourceAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        httpClient.DefaultRequestHeaders.Clear();
        
        httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        httpClient.DefaultRequestHeaders.Accept.Add(
           new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml",0.9));

        var response = await httpClient.GetAsync("api/movies");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var movies = new List<Movie>();

        if(response.Content.Headers.ContentType?.MediaType == "application/json")
        {
            movies = JsonSerializer.Deserialize<List<Movie>>(content,
                _jsonSerializerOptionsWrapper.Options);
        }
        else if (response.Content.Headers.ContentType?.MediaType == "application/xml")
        {
            var serializer = new XmlSerializer(typeof(List<Movie>));
            movies = serializer.Deserialize(new StringReader(content)) as List<Movie>;
        }

        foreach (var movie in movies)
        {
            Console.WriteLine(movie.Title);
        }  

    }

    public async Task GetResourceThroughHttpRequestMessageAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var request = new HttpRequestMessage(HttpMethod.Get,"api/movies");

        request.Headers.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var movies = JsonSerializer.Deserialize<List<Movie>>(content,
                _jsonSerializerOptionsWrapper.Options);

        foreach (var movie in movies)
        {
            Console.WriteLine(movie.Title);
        }
    }

    public async Task CreateResourceAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");
        var movieToCreate = new MovieForCreation()
        {
            Title = "Pursuit of happiness",
            Description = "dfghdfgh dfghdfgh dghdfgh",
            DirectorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
            ReleaseDate = new DateTimeOffset(new DateTime(1992, 9, 2)),
            Genre = "Drama"
        };

        var serializedMovieToCreate = JsonSerializer.Serialize(movieToCreate,
            _jsonSerializerOptionsWrapper.Options);

        var request = new HttpRequestMessage(HttpMethod.Post, "api/movies");
        request.Headers.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        request.Content = new StringContent(serializedMovieToCreate); //ObjectContent,StreamContent, ByteArrayContent
        request.Content.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var createdMovie = JsonSerializer.Deserialize<Movie>(content,
            _jsonSerializerOptionsWrapper.Options);

        Console.WriteLine(createdMovie.Title);
    }

    public async Task UpdateResourceAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var movieToUpdate = new MovieForUpdate()
        {
            Title = "Pursuit of happiness",
            Description = "Proper description",
            DirectorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
            ReleaseDate = new DateTimeOffset(new DateTime(1992, 9, 2)),
            Genre = "Drama"
        };

        var serializedMovieToUpdate = JsonSerializer.Serialize(movieToUpdate,
            _jsonSerializerOptionsWrapper.Options);

        var request = new HttpRequestMessage(
            HttpMethod.Put,
            "api/movies/436975D6-320C-4276-9926-1EBF29F9FB0B");

        request.Headers.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        request.Content = new StringContent(serializedMovieToUpdate);
        request.Content.Headers.ContentType = 
            new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var updatedMovie = JsonSerializer.Deserialize<Movie>(content,
            _jsonSerializerOptionsWrapper.Options);

        Console.WriteLine($"Title: {updatedMovie.Title}, description: {updatedMovie.Description}");

    }

    public async Task DeleteResourceAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var request = new HttpRequestMessage(
            HttpMethod.Delete,
            "api/movies/436975D6-320C-4276-9926-1EBF29F9FB0B");

        request.Headers.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
    }
}
