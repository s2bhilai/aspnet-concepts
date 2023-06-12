using Microsoft.AspNetCore.Mvc;
using Movies.Client.Helpers;
using Movies.Client.Models;
using System.Text.Json;

namespace Movies.Client.Services;

public class FaultsAndErrorsSamples : IIntegrationService
{
    private IHttpClientFactory _httpClientFactory;
    private JsonSerializerOptionsWrapper _jsonSerializationWrapper;

    public FaultsAndErrorsSamples(IHttpClientFactory httpClientFactory,
        JsonSerializerOptionsWrapper jsonSerializerOptionsWrapper)
    {
        _httpClientFactory = httpClientFactory;
        _jsonSerializationWrapper = jsonSerializerOptionsWrapper;
    }

    public async Task RunAsync()
    {
        await GetMovieAndDealWithInvalidResponsesAsync(CancellationToken.None);
        //await PostMovieAndHandleErrorsAsync(CancellationToken.None);
    }

    private async Task GetMovieAndDealWithInvalidResponsesAsync(
        CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var request = new HttpRequestMessage(HttpMethod.Get,
            $"api/movies/3d2880ae-5ba6-417c-845d-f4ebfd4bcac6");
        request.Headers.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.AcceptEncoding.Add(
            new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));

        using (var response = await httpClient.SendAsync(request,
                        HttpCompletionOption.ResponseHeadersRead,
                        cancellationToken))
        {
            if(!response.IsSuccessStatusCode)
            {
                if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine("Requested movie not found");
                    return;
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    //trigger login flow
                    return;
                }

                response.EnsureSuccessStatusCode();
            }

            var stream = await response.Content.ReadAsStreamAsync();
            var movie =  await JsonSerializer.DeserializeAsync<Movie>(stream, 
                _jsonSerializationWrapper.Options);
        }
    }

    private async Task PostMovieAndHandleErrorsAsync(CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var movieForCreation = new MovieForCreation() { };

        var serializedMovieForCreation = JsonSerializer.Serialize(movieForCreation,
            _jsonSerializationWrapper.Options);

        using(var request = new HttpRequestMessage(HttpMethod.Post,"api/movies"))
        {
            request.Headers.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(
                new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));

            request.Content = new StringContent(serializedMovieForCreation);
            request.Content.Headers.ContentType = 
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            using (var response = await httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead,cancellationToken))
            {
                if(!response.IsSuccessStatusCode)
                {
                    if(response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        var errorStream = await response.Content.ReadAsStreamAsync();

                        var errorAsProblemDetails = await JsonSerializer.DeserializeAsync<ExtendedProblemDetailsWithErrors>(errorStream,
                            _jsonSerializationWrapper.Options);

                        var errors = errorAsProblemDetails?.Errors;
                        foreach (var item in errors)
                        {
                            Console.WriteLine(item.Value[0]);
                        }
                        
                        return;
                    }
                    else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return;
                    }

                    response.EnsureSuccessStatusCode();
                }

                

                var stream = await response.Content.ReadAsStreamAsync();
                var movie = JsonSerializer.Deserialize<Movie>(stream,
                     _jsonSerializationWrapper.Options);

                Console.WriteLine(movie?.Title);
            }
        }

    }
} 