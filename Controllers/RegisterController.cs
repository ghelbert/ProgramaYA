using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProgramaYA.Models;

namespace ProgramaYA.Controllers;

using ProgramaYA.Data;
using Microsoft.EntityFrameworkCore;

public class RegisterController : Controller
{
    private readonly ILogger<RegisterController> _logger;
    private readonly ApplicationDbContext _context;

    public RegisterController(ILogger<RegisterController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }


    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(Models.Usuario usuario)
    {
        if (string.IsNullOrEmpty(usuario.Correo) || string.IsNullOrEmpty(usuario.Contrasena))
        {
            ViewBag.ErrorMessage = "Debe ingresar correo y contraseña.";
            return View("Index", usuario);
        }

        // Buscar usuario en la base de datos
        var user = _context.Set<Models.Usuario>()
            .FirstOrDefault(u => u.Correo == usuario.Correo && u.Contrasena == usuario.Contrasena);

        if (user != null)
        {
            TempData["SuccessMessage"] = "¡Bienvenido!";
            return RedirectToAction("Index", "Home");
        }
        else
        {
            ViewBag.ErrorMessage = "Correo o contraseña incorrectos.";
            return View("Index", usuario);
        }
    }


}
