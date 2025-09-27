using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramaYA.Areas.Identity.Data;
using ProgramaYA.Models;

namespace ProgramaYA.Controllers;

public class CatalogoController : Controller
{
    private readonly ILogger<CatalogoController> _logger;
    private readonly ApplicationDbContext _context;

    public CatalogoController(ILogger<CatalogoController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public string GetYouTubeVideoId(string url)
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
