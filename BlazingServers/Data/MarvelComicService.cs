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
            using (var client = new HttpClient())
            {
                string ts = DateTime.Now.Ticks.ToString();
                string hash = GenerateMd5Hash(ts, _settings.PrivateKey, _settings.PublicKey);

                string url = $"http://gateway.marvel.com/v1/public/characters?ts={ts}&apikey={_settings.PublicKey}&hash={hash}&nameStartsWith={name}";

                HttpResponseMessage response = await client.GetAsync(url);

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

                    // strip all image url from the results for an image collection
                    var imageUrls = await GetImagesFromResults(marvelResponse);

                    return marvelResponse;
                }
                else
                {
                    return new MarvelResponse.Rootobject();
                }
            }
        }
        public async Task<MarvelSeriesResponse.Result> SearchSeriesByNameAsync(string name)
        {
            using (var client = new HttpClient())
            {
                string ts = DateTime.Now.Ticks.ToString();
                string hash = GenerateMd5Hash(ts, _settings.PrivateKey, _settings.PublicKey);

                string url = $"http://gateway.marvel.com/v1/public/series?ts={ts}&apikey={_settings.PublicKey}&hash={hash}&titleStartsWith={name}";

                HttpResponseMessage response = await client.GetAsync(url);

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
        }
        public async Task<MarvelResponse.Result?> GetCharacterByIdAsync(int characterId)
        {
            using (var client = new HttpClient())
            {
                string ts = DateTime.Now.Ticks.ToString();
                string hash = GenerateMd5Hash(ts, _settings.PrivateKey, _settings.PublicKey);
                var response = await client.GetAsync($"http://gateway.marvel.com/v1/public/characters/{characterId}?ts={ts}&apikey={_settings.PublicKey}&hash={hash}");
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
            }
            return null;
        }

        public async Task<List<string>> GetImagesFromResults(MarvelResponse.Rootobject marvelResponse)
        {
            List<string> imageUrls = new List<string>();
            foreach (var result in marvelResponse.data.results)
            {
                if (result.thumbnail != null)
                {
                    // further confirm that none of the images contain 'image_not_available', as we don't want to display those
                    if (!result.thumbnail.path.Contains("image_not_available"))
                        imageUrls.Add(result.thumbnail.path + "." + result.thumbnail.extension);
                }
            }
            return imageUrls;
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
            using var _httpClient = new HttpClient();
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

        public bool IsImageAvailable(MarvelCharacterResponse.Result character)
        {
            return character.thumbnail != null && !character.thumbnail.path.Contains("image_not_available");
        }

        public async Task<MarvelCharacterResponse.Result?> GetRandomCharacterAsync()
        {
            using var _httpClient = new HttpClient();
            string ts = DateTime.Now.Ticks.ToString();
            string hash = GenerateMd5Hash(ts, _settings.PrivateKey, _settings.PublicKey);

            MarvelCharacterResponse.Result? character = null;

            do
            {
                int offset = new Random().Next(0, 1000); // Adjust as needed for your data size
                var url = $"http://gateway.marvel.com/v1/public/characters?limit=1&offset={offset}&ts={ts}&apikey={_settings.PublicKey}&hash={hash}";
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
                    character = jsonResponse?.data.results.FirstOrDefault();
                }

            } while (character != null && !IsImageAvailable(character));

            return character;
        }


        public async Task<MarvelSeriesResponse.Result?> GetSeriesByIdAsync(int seriesId)
        {
            using var _httpClient = new HttpClient();
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
            using var _httpClient = new HttpClient();
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
            using var _httpClient = new HttpClient();
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
            using var _httpClient = new HttpClient();
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
