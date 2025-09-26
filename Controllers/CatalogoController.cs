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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
