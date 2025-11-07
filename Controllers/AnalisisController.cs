using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using apptienda.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProgramaYA.Areas.Identity.Data;

namespace Repositorio.Controllers
{
    public class AnalisisController : Controller
    {
        private readonly ILogger<AnalisisController> _logger;
        private readonly ApplicationDbContext _context;

        public AnalisisController(ILogger<AnalisisController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var topCursos = await _context.Comentarios
       .Where(c => c.CursoSeleccionado != null && c.Etiqueta == "Positivo")
       .GroupBy(c => c.CursoSeleccionado)
       .Select(g => new
       {
           CursoId = g.Key,
           Positivos = g.Count()
       })
       .OrderByDescending(g => g.Positivos)
       .Take(3)
       .ToListAsync();

            var downCursos = await _context.Comentarios
       .Where(c => c.CursoSeleccionado != null && c.Etiqueta == "Negativo")
       .GroupBy(c => c.CursoSeleccionado)
       .Select(g => new
       {
           CursoId = g.Key,
           Negativos = g.Count()
       })
       .OrderByDescending(g => g.Negativos)
       .Take(3)
       .ToListAsync();

            // Obtener nombres de los cursos (si existe navegación o tabla Curso)
            var cursos = await _context.Cursos
                .Where(c => topCursos.Select(t => t.CursoId).Contains(c.Id))
                .ToListAsync();

            var cursos2 = await _context.Cursos
                .Where(c => downCursos.Select(t => t.CursoId).Contains(c.Id))
                .ToListAsync();

            // Unir resultados (curso + cantidad de positivos)
            var resultado = topCursos
                .Join(cursos,
                      t => t.CursoId,
                      c => c.Id,
                      (t, c) => new
                      {
                          Curso = c.Nombre, // cambia a la propiedad real del curso
                          Positivos = t.Positivos
                      })
                .ToList();

            var resultado2 = downCursos
                .Join(cursos2,
                      t => t.CursoId,
                      c => c.Id,
                      (t, c) => new
                      {
                          Curso = c.Nombre, // cambia a la propiedad real del curso
                          Negativos = t.Negativos
                      })
                .ToList();

            // Enviar datos a la vista
            ViewBag.Cursos = resultado.Select(r => r.Curso).ToList();
            ViewBag.Positivos = resultado.Select(r => r.Positivos).ToList();
            ViewBag.Cursos2 = resultado2.Select(r => r.Curso).ToList();
            ViewBag.Negativos = resultado2.Select(r => r.Negativos).ToList();

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}