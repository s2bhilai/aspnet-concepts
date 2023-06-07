using Microsoft.AspNetCore.JsonPatch;
using Movies.Client.Models;
using Newtonsoft.Json;
using System.Text;

namespace Movies.Client.Services;

public class PartialUpdateSamples : IIntegrationService
{
    private IHttpClientFactory _httpClientFactory;

    public PartialUpdateSamples(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? 
            throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task RunAsync()
    {
        //await PatchResourceAsync();
        await PatchResourceShortcutAsync();
    }

    public async Task PatchResourceAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");
        var patchDoc = new JsonPatchDocument<MovieForUpdate>();

        patchDoc.Replace(r => r.Title, "Updated Title via Json Patch");
        patchDoc.Remove(r => r.Description);

        var serializedChangeSet = JsonConvert.SerializeObject(patchDoc);

        var request =new HttpRequestMessage(
            HttpMethod.Patch, "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b");

        request.Headers.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        request.Content = new StringContent(serializedChangeSet);
        request.Content.Headers.ContentType = 
            new System.Net.Http.Headers.MediaTypeHeaderValue("application/json-patch+json");

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var updatedMovie = JsonConvert.DeserializeObject<Movie>(content);

        Console.WriteLine($"Title: {updatedMovie.Title}, Description: {updatedMovie.Description}");

    }

    public async Task PatchResourceShortcutAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var patchDoc = new JsonPatchDocument<MovieForUpdate>();
        patchDoc.Replace(r => r.Title, "Updated Title via Json Patch Shortcut");

        var response = await httpClient.PatchAsync(
            "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b",
            new StringContent(JsonConvert.SerializeObject(patchDoc),
                Encoding.UTF8, "application/json-patch+json"));

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var updatedMovie = JsonConvert.DeserializeObject<Movie>(content);
    }
}
