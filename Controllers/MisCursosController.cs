

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
    public async Task<IActionResult> Eliminar(int id)
    {
        if (!User?.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            return Forbid();
        var curso = await _context.Cursos.FindAsync(id);
        if (curso == null)
            return NotFound();
        return View(curso);
    }

    [HttpPost, ActionName("Eliminar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarConfirmado(int id)
    {
        if (!User?.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            return Forbid();
        var curso = await _context.Cursos.FindAsync(id);
        if (curso == null)
            return NotFound();
        _context.Cursos.Remove(curso);
        await _context.SaveChangesAsync();
        return RedirectToAction("Todos");
    }
    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        if (!User?.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            return Forbid();
        var curso = await _context.Cursos.FindAsync(id);
        if (curso == null)
            return NotFound();
        return View(curso);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(Curso curso)
    {
        if (!User?.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            return Forbid();
        if (ModelState.IsValid)
        {
            _context.Update(curso);
            await _context.SaveChangesAsync();
            return RedirectToAction("Todos");
        }
        return View(curso);
    }

    [HttpGet]
    public IActionResult Agregar()
    {
        if (!User?.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            return Forbid();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Agregar(Curso curso)
    {
        if (!User?.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            return Forbid();
        if (ModelState.IsValid)
        {
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();
            return RedirectToAction("Todos");
        }
        return View(curso);
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
}
