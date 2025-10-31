using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProgramaYA.Areas.Identity.Data;
using ProgramaYA.Models;

namespace Repositorio.Controllers
{
    public class MisClientesController : Controller
    {
        private readonly ILogger<MisClientesController> _logger;
        private readonly ApplicationDbContext _context;

        public MisClientesController(ILogger<MisClientesController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<ApplicationUser> lista = await _context.Users.ToListAsync();
            return View(lista);
        }
        [HttpGet]
        public async Task<IActionResult> Editar(string id)
        {
            if (!User?.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
                return Forbid();
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(ApplicationUser usuario)
        {
            if (!User?.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
                return Forbid();
            var usuarioExistente = await _context.Users.FindAsync(usuario.Id);

            if (ModelState.IsValid)
            {
                usuarioExistente.Nombres = usuario.Nombres;
                usuarioExistente.Apellidos = usuario.Apellidos;
                usuarioExistente.DNI = usuario.DNI;
                usuarioExistente.NormalizedUserName = usuario.NormalizedUserName;
                usuarioExistente.Celular = usuario.Celular;
                await _context.SaveChangesAsync();
            }


            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}