using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ProgramaYA.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<CurrencyService> _logger;

        public CurrencyService(HttpClient httpClient, IConfiguration configuration, ILogger<CurrencyService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["GetGeoApi:ApiKey"] ?? string.Empty;
        }

        public async Task<Dictionary<string, string>> GetCurrenciesAsync()
        {
            var result = new Dictionary<string, string>();
            try
            {
                var url = $"currency/list?api_key={_apiKey}&format=json";
                var resp = await _httpClient.GetAsync(url);
                var body = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode)
                {
                    _logger.LogError("GetGeoAPI currencies list failed {Status}: {Body}", (int)resp.StatusCode, body);
                    return result;
                }

                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;
                if (root.TryGetProperty("currencies", out var currenciesEl) && currenciesEl.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in currenciesEl.EnumerateObject())
                    {
                        result[prop.Name] = prop.Value.GetString() ?? string.Empty;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while fetching currencies list");
                return result;
            }
        }

        public async Task<(bool Success, decimal? Result, string RawMessage)> ConvertAsync(string from, string to, decimal amount)
        {
            try
            {
                var url = $"currency/convert?api_key={_apiKey}&from={Uri.EscapeDataString(from)}&to={Uri.EscapeDataString(to)}&amount={amount}&format=json";
                var resp = await _httpClient.GetAsync(url);
                var body = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode)
                {
                    _logger.LogError("GetGeoAPI convert failed {Status}: {Body}", (int)resp.StatusCode, body);
                    return (false, null, body);
                }

                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                // Try several patterns for result extraction
                if (root.TryGetProperty("result", out var resultEl) && resultEl.ValueKind == JsonValueKind.Number)
                {
                    return (true, resultEl.GetDecimal(), body);
                }

                if (root.TryGetProperty("rates", out var ratesEl) && ratesEl.ValueKind == JsonValueKind.Object)
                {
                    if (ratesEl.TryGetProperty(to, out var rateEl))
                    {
                        if (rateEl.ValueKind == JsonValueKind.Number)
                        {
                            return (true, rateEl.GetDecimal(), body);
                        }
                        else if (rateEl.ValueKind == JsonValueKind.Object)
                        {
                            // Some responses contain numeric properties as strings, e.g. "rate":"0.8604" or "rate_for_amount":"104.9679"
                            if (rateEl.TryGetProperty("rate_for_amount", out var rfa))
                            {
                                if (rfa.ValueKind == JsonValueKind.Number)
                                {
                                    return (true, rfa.GetDecimal(), body);
                                }
                                if (rfa.ValueKind == JsonValueKind.String && decimal.TryParse(rfa.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedRfa))
                                {
                                    return (true, parsedRfa, body);
                                }
                            }

                            if (rateEl.TryGetProperty("rate", out var r) )
                            {
                                if (r.ValueKind == JsonValueKind.Number)
                                {
                                    return (true, r.GetDecimal() * amount, body);
                                }
                                if (r.ValueKind == JsonValueKind.String && decimal.TryParse(r.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedR))
                                {
                                    return (true, parsedR * amount, body);
                                }
                            }

                            if (rateEl.TryGetProperty("rate_float", out var rf) && rf.ValueKind == JsonValueKind.Number)
                            {
                                return (true, rf.GetDecimal() * amount, body);
                            }
                        }
                    }
                }

                // Some APIs return 'amount' and 'base' and 'converted' fields
                if (root.TryGetProperty("amount", out var amountEl) && root.TryGetProperty("to", out var toEl))
                {
                    // fallback: try to find any numeric property named like the target currency
                    if (root.TryGetProperty(to, out var toVal) && toVal.ValueKind == JsonValueKind.Number)
                    {
                        return (true, toVal.GetDecimal(), body);
                    }
                }

                // As a last resort, try to find the first numeric value in the document
                decimal? found = null;
                void Search(JsonElement el)
                {
                    if (found.HasValue) return;
                    switch (el.ValueKind)
                    {
                        case JsonValueKind.Number:
                            found = el.GetDecimal();
                            return;
                        case JsonValueKind.Object:
                            foreach (var p in el.EnumerateObject()) Search(p.Value);
                            break;
                        case JsonValueKind.Array:
                            foreach (var v in el.EnumerateArray()) Search(v);
                            break;
                    }
                }

                Search(root);
                return (found.HasValue, found, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while converting currency");
                return (false, null, ex.Message);
            }
        }
    }
}
