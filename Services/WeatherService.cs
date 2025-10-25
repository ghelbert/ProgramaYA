using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProgramaYA.Models.Dto;

namespace ProgramaYA.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<WeatherService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<(bool Success, WeatherData Data, string Raw, string Error)> GetCurrentAsync(decimal lat, decimal lon, string units = "metric", string lang = "es")
        {
            try
            {
                var url = $"current?lon={Uri.EscapeDataString(lon.ToString())}&lat={Uri.EscapeDataString(lat.ToString())}&units={Uri.EscapeDataString(units)}&lang={Uri.EscapeDataString(lang)}";
                var resp = await _httpClient.GetAsync(url);
                var body = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode)
                {
                    _logger.LogError("Weather API returned {Status}: {Body}", (int)resp.StatusCode, body);
                    return (false, null, body, $"Status {(int)resp.StatusCode}");
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<WeatherResponse>(body, options);
                var first = result?.Data != null && result.Data.Count > 0 ? result.Data[0] : null;
                return (true, first, body, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while calling Weather API");
                return (false, null, ex.Message, ex.Message);
            }
        }
    }
}
