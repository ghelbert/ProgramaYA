using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ProgramaYA.Areas.Identity.Data;
using ProgramaYA.Models;

namespace ProgramaYA.Controllers;

public class CatalogoController : Controller
{
    private readonly ILogger<CatalogoController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly UserManager<ApplicationUser> _userManager;

    public CatalogoController(UserManager<ApplicationUser> userManager, ILogger<CatalogoController> logger, ApplicationDbContext context, IDistributedCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        List<Curso>? cursos = await _context.Cursos.ToListAsync();
        ViewData["Title"] = "Lista de Cursos";
        return View(cursos);
    }

    public async Task<IActionResult> Detalles(int id)
    {
        Curso? curso = await _context.Cursos
        .Include(i => i.Capitulos)
        .FirstOrDefaultAsync(i => i.Id == id);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != null)
        {
            string cacheKey = $"curso:{userId}";
            var cachedBytes = _cache.GetAsync(cacheKey).GetAwaiter().GetResult();
            List<Curso> CursosRedis = new();
            if (curso != null)
            {
                if (cachedBytes != null && cachedBytes.Length > 0)
                {
                    var json = Encoding.UTF8.GetString(cachedBytes);
                    CursosRedis = JsonSerializer.Deserialize<List<Curso>>(json) ?? new List<Curso>();
                    // Si ya hay 3 cursos en el historial, eliminamos el más antiguo (al inicio)
                    if (CursosRedis.Count >= 3)
                    {
                        CursosRedis.RemoveAt(0);
                    }

                    // Agregamos el nuevo curso al final
                    CursosRedis.Add(curso);
                    var json2 = JsonSerializer.Serialize(CursosRedis);
                    var bytes = Encoding.UTF8.GetBytes(json2);
                    var options = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(120)
                    };
                    _cache.SetAsync(cacheKey, bytes, options).GetAwaiter().GetResult();
                }
                else
                {
                    CursosRedis.Add(curso);
                    var json = JsonSerializer.Serialize(CursosRedis);
                    var bytes = Encoding.UTF8.GetBytes(json);
                    var options = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(120)
                    };
                    _cache.SetAsync(cacheKey, bytes, options).GetAwaiter().GetResult();
                }
            }
        }

        foreach (var item in curso.Capitulos)
        {
            item.Video = GetYouTubeVideoId(item.Video);
        }
        if (curso == null)
        {
            return NotFound();
        }
        return View(curso);
    }
    public async Task<IActionResult> Suscripcion(int id)
    {
        Curso? curso = await _context.Cursos
        .FirstOrDefaultAsync(i => i.Id == id);

        if (curso == null)
        {
            return NotFound();
        }
        return View(curso);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public static string GetYouTubeVideoId(string url)
    {
        if (string.IsNullOrEmpty(url)) return string.Empty;

        var uri = new Uri(url);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        var videoId = query["v"];

        // Si no está en formato adecuado, buscar ID en formato alternativo
        if (string.IsNullOrEmpty(videoId))
        {
            var splitUrl = url.Split('/');
            videoId = splitUrl.LastOrDefault();
        }

        return videoId;
    }
}
