using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ProgramaYA.Models;

namespace ProgramaYA.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDistributedCache _cacheRedis;

    public HomeController(ILogger<HomeController> logger, IDistributedCache cacheRedis)
    {
        _logger = logger;
        _cacheRedis = cacheRedis;
    }

    public IActionResult Index()
    {
        if (User?.Identity?.IsAuthenticated == true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cacheKey = $"curso:{userId}";
            var cachedBytes = _cacheRedis.GetAsync(cacheKey).GetAwaiter().GetResult();
            if (cachedBytes != null && cachedBytes.Length > 0)
            {
                var json = Encoding.UTF8.GetString(cachedBytes);
                List<Curso> CursosRedis = JsonSerializer.Deserialize<List<Curso>>(json) ?? new List<Curso>();
                return View(CursosRedis);
            }
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
