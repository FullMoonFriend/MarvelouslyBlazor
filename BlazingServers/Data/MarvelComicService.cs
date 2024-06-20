using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace BlazingServers.Data
{
    public class MarvelComicService
    {
        private readonly HttpClient _httpClient;
        private readonly MarvelApiSettings _settings;

        public MarvelComicService(HttpClient httpClient, IOptions<MarvelApiSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }
        public string GenerateMd5Hash(string timestamp, string privateKey, string publicKey)
        {
            using var md5 = MD5.Create();
            var input = $"{timestamp}{privateKey}{publicKey}";
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString().ToLower();
        }
        public async Task<MarvelResponse.Rootobject> SearchComicByNameAsync(string name)
        {

                string ts = DateTime.Now.Ticks.ToString();
                string hash = GenerateMd5Hash(ts, _settings.PrivateKey, _settings.PublicKey);

                string url = $"http://gateway.marvel.com/v1/public/characters?ts={ts}&apikey={_settings.PublicKey}&hash={hash}&nameStartsWith={name}";

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        Converters =
                        {
                            new CustomDateTimeConverter()
                        }
                    };
                    // Deserialize the data into MarvelResponse.Rootobject
                    var marvelResponse = JsonSerializer.Deserialize<MarvelResponse.Rootobject>(data, options);

                    return marvelResponse;
                }
                else
                {
                    return new MarvelResponse.Rootobject();
                }
            
        }
        public async Task<MarvelSeriesResponse.Result> SearchSeriesByNameAsync(string name)
        {
                string ts = DateTime.Now.Ticks.ToString();
                string hash = GenerateMd5Hash(ts, _settings.PrivateKey, _settings.PublicKey);

                string url = $"http://gateway.marvel.com/v1/public/series?ts={ts}&apikey={_settings.PublicKey}&hash={hash}&titleStartsWith={name}";

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        Converters =
                        {
                            new CustomDateTimeConverter()
                        }
                    };
                    // Deserialize the data
                    var marvelResponse = JsonSerializer.Deserialize<MarvelSeriesResponse.Rootobject>(data, options);
                    return marvelResponse.data.results.FirstOrDefault();
                }
                else
                {
                    return new MarvelSeriesResponse.Result();
                }
        }
        public async Task<MarvelResponse.Result?> GetCharacterByIdAsync(int characterId)
        {

                string ts = DateTime.Now.Ticks.ToString();
                string hash = GenerateMd5Hash(ts, _settings.PrivateKey, _settings.PublicKey);
                var response = await _httpClient.GetAsync($"http://gateway.marvel.com/v1/public/characters/{characterId}?ts={ts}&apikey={_settings.PublicKey}&hash={hash}");
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        Converters =
                            {
                                new CustomDateTimeConverter()
                            }
                    };
                    var responseString = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonSerializer.Deserialize<MarvelResponse.Rootobject>(responseString, options);
                    return jsonResponse?.data.results.FirstOrDefault();
                }
            return null;
        }

        public async Task<MarvelComicResponse.Result?> GetComicByIdAsync(int comicId)
        {
            using var _httpClient = new HttpClient();
            string ts = DateTime.Now.Ticks.ToString();
            string hash = GenerateMd5Hash(ts, _settings.PrivateKey, _settings.PublicKey);
            var response = await _httpClient.GetAsync($"http://gateway.marvel.com/v1/public/comics/{comicId}?ts={ts}&apikey={_settings.PublicKey}&hash={hash}");
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    Converters =
                            {
                                new CustomDateTimeConverter()
                            }
                };
                var responseString = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<MarvelComicResponse.Rootobject>(responseString, options);
                return jsonResponse?.data.results.FirstOrDefault();
            }
            return null;
        }

        public async Task<MarvelSeriesResponse.Result?> GetRandomSeriesAsync()
        {
            using var _httpClient = new HttpClient();
            string ts = DateTime.Now.Ticks.ToString();
            string hash = GenerateMd5Hash(ts, _settings.PrivateKey, _settings.PublicKey);

            MarvelSeriesResponse.Result? series = null;

            do
            {
                int offset = new Random().Next(0, 1000); // Adjust as needed for your data size
                var url = $"http://gateway.marvel.com/v1/public/series?limit=1&offset={offset}&ts={ts}&apikey={_settings.PublicKey}&hash={hash}";
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        Converters =
                {
                    new CustomDateTimeConverter()
                }
                    };
                    var responseString = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonSerializer.Deserialize<MarvelSeriesResponse.Rootobject>(responseString, options);
                    series = jsonResponse?.data.results.FirstOrDefault();
                }

            } while (series != null && !IsImageAvailable(series));

            return series;
        }

        public bool IsImageAvailable(MarvelSeriesResponse.Result series)
        {
            return series.thumbnail != null && !series.thumbnail.path.Contains("image_not_available");
        }


        public async Task<List<MarvelCharacterResponse.Result>> GetCharactersBySeriesAsync(int seriesId)
        {
            string ts = DateTime.Now.Ticks.ToString();
            string hash = GenerateMd5Hash(ts, _settings.PrivateKey, _settings.PublicKey);
            var url = $"http://gateway.marvel.com/v1/public/series/{seriesId}/characters?ts={ts}&apikey={_settings.PublicKey}&hash={hash}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    Converters =
                    {
                        new CustomDateTimeConverter()
                    }
                };
                var responseString = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<MarvelCharacterResponse.Rootobject>(responseString, options);
                return jsonResponse?.data.results.ToList();
            }
            return new List<MarvelCharacterResponse.Result>();
        }
        public async Task<List<MarvelCharacterResponse.Result>> GetRandomCharactersAsync(int numberOfCharacters)
        {
            string ts = DateTime.Now.Ticks.ToString();
            string hash = GenerateMd5Hash(ts, _settings.PrivateKey, _settings.PublicKey);

            List<MarvelCharacterResponse.Result> characters = new List<MarvelCharacterResponse.Result>();
            int retries = 0;
            const int maxRetries = 5; // Limit the number of retries to prevent infinite loops

            while (characters.Count < numberOfCharacters && retries < maxRetries)
            {
                int offset = new Random().Next(0, 1564); // Adjust as needed for your data size
                var url = $"http://gateway.marvel.com/v1/public/characters?limit=10&offset={offset}&ts={ts}&apikey={_settings.PublicKey}&hash={hash}";
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        Converters =
                {
                    new CustomDateTimeConverter()
                }
                    };
                    var responseString = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonSerializer.Deserialize<MarvelCharacterResponse.Rootobject>(responseString, options);
                    if (jsonResponse != null)
                    {
                        var validCharacters = jsonResponse.data.results.Where(IsImageAvailable).ToList();
                        characters.AddRange(validCharacters);
                        // Break the loop if we have enough characters
                        if (characters.Count >= numberOfCharacters)
                        {
                            break;
                        }
                    }
                }
                retries++;
            }

            return characters.Take(numberOfCharacters).ToList(); // Ensure we return only the requested number of characters
        }

        public bool IsImageAvailable(MarvelCharacterResponse.Result character)
        {
            return character.thumbnail != null && !character.thumbnail.path.Contains("image_not_available");
        }


        public async Task<MarvelSeriesResponse.Result?> GetSeriesByIdAsync(int seriesId)
        {
            string ts = DateTime.Now.Ticks.ToString();
            string hash = GenerateMd5Hash(ts, _settings.PrivateKey, _settings.PublicKey);
            var url = $"http://gateway.marvel.com/v1/public/series/{seriesId}?ts={ts}&apikey={_settings.PublicKey}&hash={hash}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    Converters =
                            {
                                new CustomDateTimeConverter()
                            }
                };
                var responseString = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<MarvelSeriesResponse.Rootobject>(responseString, options);
                return jsonResponse?.data.results.FirstOrDefault();
            }
            return null;
        }
        public async Task<MarvelCreatorResponse.Result?> GetCreatorByIdAsync(int creatorId)
        {
            string ts = DateTime.Now.Ticks.ToString();
            string hash = GenerateMd5Hash(ts, _settings.PrivateKey, _settings.PublicKey);
            var url = $"http://gateway.marvel.com/v1/public/creators/{creatorId}?ts={ts}&apikey={_settings.PublicKey}&hash={hash}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    Converters =
                            {
                                new CustomDateTimeConverter()
                            }
                };
                var responseString = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<MarvelCreatorResponse.Rootobject>(responseString, options);
                return jsonResponse?.data.results.FirstOrDefault();
            }
            return null;
        }

        public async Task<MarvelStoriesResponse.Result?> GetStoryByIdAsync(int storyId)
        {
            string ts = DateTime.Now.Ticks.ToString();
            string hash = GenerateMd5Hash(ts, _settings.PrivateKey, _settings.PublicKey);
            var url = $"http://gateway.marvel.com/v1/public/stories/{storyId}?ts={ts}&apikey={_settings.PublicKey}&hash={hash}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    Converters =
                            {
                                new CustomDateTimeConverter()
                            }
                };
                var responseString = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<MarvelStoriesResponse.Rootobject>(responseString, options);
                return jsonResponse?.data.results.FirstOrDefault();
            }
            return null;
        }

        public async Task<MarvelEventResponse.Result?> GetEventByIdAsync(int eventId)
        {
            string ts = DateTime.Now.Ticks.ToString();
            string hash = GenerateMd5Hash(ts, _settings.PrivateKey, _settings.PublicKey);
            var url = $"http://gateway.marvel.com/v1/public/events/{eventId}?ts={ts}&apikey={_settings.PublicKey}&hash={hash}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    Converters =
                            {
                                new CustomDateTimeConverter()
                            }
                };
                var responseString = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<MarvelEventResponse.Root>(responseString, options);
                return jsonResponse?.data.results.FirstOrDefault();
            }
            return null;
        }


    }
}
