using Movies.Client.Helpers;
using Movies.Client.Models;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Movies.Client.Services;

public class CompressionSamples : IIntegrationService
{
    private IHttpClientFactory _httpClientFactory;
    private JsonSerializerOptionsWrapper _jsonSerializationOptions;

    public CompressionSamples(IHttpClientFactory httpClientFactory,
        JsonSerializerOptionsWrapper jsonSerializerOptionsWrapper)
    {
        _httpClientFactory = httpClientFactory;
        _jsonSerializationOptions = jsonSerializerOptionsWrapper;
    }


    public async Task RunAsync()
    {
        //await GetPosterWithGZipCompressionAsync();
        await SendAndReceiveWithGZipCompressionAsync();
    }

    private async Task GetPosterWithGZipCompressionAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var request = new HttpRequestMessage(HttpMethod.Get,
            $"api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b/posters/{Guid.NewGuid()}");
        request.Headers.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        //Header for encoding
        request.Headers.AcceptEncoding.Add(
            new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));

        using(var response = await httpClient.SendAsync(request,
            HttpCompletionOption.ResponseHeadersRead))
        {
            var stream = await response.Content.ReadAsStreamAsync();
            response.EnsureSuccessStatusCode();

            var poster = await JsonSerializer.DeserializeAsync<Poster>(stream,
                _jsonSerializationOptions.Options);

            Console.WriteLine($"Name: {poster.Name}, ID:{poster.Id}");

        }
    }

    private async Task SendAndReceiveWithGZipCompressionAsync()
    {
        var client = _httpClientFactory.CreateClient("MoviesAPIClient");

        var random = new Random();
        var generatedBytes = new byte[5242880];
        random.NextBytes(generatedBytes);

        var posterForCreation = new PosterForCreation()
        {
            Name = "A New Poster - GZip",
            Bytes = generatedBytes
        };

        using (var memoryContentStream = new MemoryStream())
        {
            //Convert the entity to memory stream
            await JsonSerializer.SerializeAsync(memoryContentStream, posterForCreation);

            //set the position to start so that data is read from start
            memoryContentStream.Seek(0, SeekOrigin.Begin);

            using (var request = new HttpRequestMessage(HttpMethod.Post, 
                "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b/posters"))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

                //Setting the content for request as stream
                using (var compressedMemoryStreamContent = new MemoryStream())
                {
                    using(var gzipStream = new GZipStream(compressedMemoryStreamContent,
                                                     CompressionMode.Compress))
                    {
                        memoryContentStream.CopyTo(gzipStream);
                        gzipStream.Flush();
                        compressedMemoryStreamContent.Position = 0;

                        using(var streamContent = new StreamContent(compressedMemoryStreamContent))
                        {
                            streamContent.Headers.ContentType =
                                    new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                            streamContent.Headers.ContentEncoding.Add("gzip");

                            request.Content = streamContent;

                            var response = await client.SendAsync(request,HttpCompletionOption.ResponseHeadersRead);
                            response.EnsureSuccessStatusCode();

                            var stream = await response.Content.ReadAsStreamAsync();
                            var poster = await JsonSerializer.DeserializeAsync<Poster>(stream,
                                                                _jsonSerializationOptions.Options);

                            Console.WriteLine($"poster name: {poster.Name}, posterids: {poster.Id}");
                        }
                    }
                }
            }
        }

    }
}

