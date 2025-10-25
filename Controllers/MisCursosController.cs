using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramaYA.Areas.Identity.Data;
using ProgramaYA.Models;

namespace ProgramaYA.Controllers;

public class MisCursosController : Controller
{
    private readonly ILogger<MisCursosController> _logger;

    private readonly ApplicationDbContext _context;

    public MisCursosController(ILogger<MisCursosController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Todos()
    {
        // Solo permitir acceso a administradores
        if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
        {
            return Forbid();
        }
        var cursos = await _context.Cursos.ToListAsync();
        return View(cursos);
    }


    public async Task<IActionResult> Index(string userId)
    {
        var cursoIds = await _context.Suscripciones
            .Where(s => s.UsuarioId == userId)
            .Select(s => s.CursoId)
            .ToListAsync();

        var cursos = await _context.Cursos
            .Where(c => cursoIds.Contains(c.Id))
            .ToListAsync();

        return View(cursos);
    }
    public async Task<IActionResult> Detalle(int id)
    {
        Curso? curso = await _context.Cursos
        .Include(i => i.Capitulos)
        .FirstOrDefaultAsync(i => i.Id == id);
        foreach (var item in curso.Capitulos)
        {
            item.Video = CatalogoController.GetYouTubeVideoId(item.Video);
        }
        if (curso == null)
        {
            return NotFound();
        }
        return View(curso);
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
