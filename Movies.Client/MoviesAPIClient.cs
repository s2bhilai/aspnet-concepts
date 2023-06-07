using Movies.Client.Helpers;
using Movies.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Movies.Client
{
    //Dont override the custom client handler here, bcoz in that case handler will not come from pool anymore.
    public class MoviesAPIClient
    {
        private JsonSerializerOptionsWrapper _jsonSerializerOptions;
        private HttpClient Client { get; }

        public MoviesAPIClient(HttpClient client, 
            JsonSerializerOptionsWrapper jsonSerializerOptionsWrapper)
        {
            Client = client;
            Client.BaseAddress = new Uri("http://localhost:5001");
            Client.Timeout = new TimeSpan(0, 0, 30);
            _jsonSerializerOptions = jsonSerializerOptionsWrapper;
        }

        public async Task<IEnumerable<Movie>?> GetMoviesAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/movies");
            request.Headers.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await Client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<IEnumerable<Movie>>(content,
                _jsonSerializerOptions.Options);
        }

    }
}
