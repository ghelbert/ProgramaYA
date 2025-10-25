using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProgramaYA.Models.Dto;

namespace ProgramaYA.Services
{
    public class NewsApiService : INewsApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<NewsApiService> _logger;

        public NewsApiService(HttpClient httpClient, IConfiguration configuration, ILogger<NewsApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["NewsApi:ApiKey"] ?? string.Empty;
        }
    
        public async Task<NewsApiResponse> GetEverythingAsync(string query, int pageSize = 20)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException(nameof(query));

            try
            {
                // NewsAPI supports apiKey as query parameter or X-Api-Key header. We'll use query parameter for simplicity.
                var url = $"everything?q={Uri.EscapeDataString(query)}&pageSize={pageSize}&apiKey={_apiKey}";
                var resp = await _httpClient.GetAsync(url);
                if (!resp.IsSuccessStatusCode)
                {
                    // Log status and body to help diagnose 400/401/429 errors from the NewsAPI
                    var body = await resp.Content.ReadAsStringAsync();
                    _logger.LogError("NewsAPI returned {StatusCode}: {Body}", (int)resp.StatusCode, body);
                    return new NewsApiResponse { Articles = new System.Collections.Generic.List<Article>() };
                }

                await using var stream = await resp.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = await JsonSerializer.DeserializeAsync<NewsApiResponse>(stream, options);
                return result ?? new NewsApiResponse { Articles = new System.Collections.Generic.List<Article>() };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error requesting NewsAPI");
                return new NewsApiResponse { Articles = new System.Collections.Generic.List<Article>() };
            }
        }
    }
}
