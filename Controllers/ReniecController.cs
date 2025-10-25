using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProgramaYA.Controllers
{
    [Route("api/reniec")]
    [ApiController]
    public class ReniecController : ControllerBase
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ReniecController> _logger;

        public ReniecController(IHttpClientFactory httpFactory, IConfiguration configuration, ILogger<ReniecController> logger)
        {
            _httpFactory = httpFactory;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("dni")]
        public async Task<IActionResult> GetByDni([FromQuery] string numero)
        {
            if (string.IsNullOrWhiteSpace(numero)) return BadRequest(new { error = "numero is required" });

            try
            {
                var token = _configuration["Decolecta:Token"];
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Decolecta token not configured");
                    return StatusCode(500, new { error = "Server not configured" });
                }

                var client = _httpFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.decolecta.com/v1/reniec/dni?numero={numero}");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var resp = await client.SendAsync(request);
                var body = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode)
                {
                    _logger.LogError("RENIEC proxy returned {Status}: {Body}", (int)resp.StatusCode, body);
                    return StatusCode((int)resp.StatusCode, new { error = "External API error", detail = body });
                }

                // Try to parse common fields nombres and apellidos
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;
                string nombres = string.Empty;
                string apellidos = string.Empty;


                // Known field variants
                if (root.TryGetProperty("nombres", out var n)) nombres = n.GetString() ?? string.Empty;
                if (root.TryGetProperty("nombres_completos", out var nc) && string.IsNullOrEmpty(nombres)) nombres = nc.GetString() ?? string.Empty;
                if (root.TryGetProperty("nombre", out var nom) && string.IsNullOrEmpty(nombres)) nombres = nom.GetString() ?? string.Empty;

                if (root.TryGetProperty("first_name", out var fn) && string.IsNullOrEmpty(nombres)) nombres = fn.GetString() ?? string.Empty;

                // Apellidos: several APIs use different names
                if (root.TryGetProperty("apellidos", out var a)) apellidos = a.GetString() ?? string.Empty;
                if (root.TryGetProperty("apellido_paterno", out var ap) && string.IsNullOrEmpty(apellidos)) apellidos = (ap.GetString() ?? string.Empty);
                if (root.TryGetProperty("apellido_materno", out var am) && string.IsNullOrEmpty(apellidos))
                {
                    var ams = am.GetString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(nombres) && string.IsNullOrEmpty(apellidos)) apellidos = ams; // fallback
                }

                // Rapid/other API fields
                if (root.TryGetProperty("first_last_name", out var fl) || root.TryGetProperty("last_name", out fl))
                {
                    var fls = fl.GetString() ?? string.Empty;
                    // combine with second_last_name if present
                    if (root.TryGetProperty("second_last_name", out var sl))
                    {
                        var sls = sl.GetString() ?? string.Empty;
                        apellidos = string.Join(' ', new[] { fls, sls }.Where(s => !string.IsNullOrEmpty(s))).Trim();
                    }
                    else if (string.IsNullOrEmpty(apellidos)) apellidos = fls;
                }

                // full_name fallback: try to split into apellidos and nombres
                if ((string.IsNullOrEmpty(nombres) || string.IsNullOrEmpty(apellidos)) && root.TryGetProperty("full_name", out var full))
                {
                    var fullName = full.GetString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(fullName))
                    {
                        // many RENIEC full_name formats are "LASTNAME1 LASTNAME2 FIRST NAMES"
                        var parts = fullName.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 3)
                        {
                            // assume first two are last names
                            var lastNames = parts.Take(2);
                            var firstNames = parts.Skip(2);
                            if (string.IsNullOrEmpty(apellidos)) apellidos = string.Join(' ', lastNames);
                            if (string.IsNullOrEmpty(nombres)) nombres = string.Join(' ', firstNames);
                        }
                    }
                }

                // If still empty, try to inspect nested object 'data' or similar
                if ((string.IsNullOrEmpty(nombres) || string.IsNullOrEmpty(apellidos)) && root.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in root.EnumerateObject())
                    {
                        if (prop.Value.ValueKind == JsonValueKind.Object)
                        {
                            var obj = prop.Value;
                            if (obj.TryGetProperty("nombres", out var nn) && string.IsNullOrEmpty(nombres)) nombres = nn.GetString() ?? string.Empty;
                            if (obj.TryGetProperty("apellidos", out var aa) && string.IsNullOrEmpty(apellidos)) apellidos = aa.GetString() ?? string.Empty;
                        }
                    }
                }

                // Final fallback: return raw body so client can inspect
                // Avoid returning JsonElement tied to a disposed JsonDocument by deserializing into a standalone JsonElement
                var rawElement = JsonSerializer.Deserialize<System.Text.Json.JsonElement>(body);
                return Ok(new { nombres, apellidos, raw = rawElement });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling RENIEC");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
