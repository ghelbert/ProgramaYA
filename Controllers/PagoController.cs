using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramaYA.Areas.Identity.Data;
using ProgramaYA.Models;

namespace ProgramaYA.Controllers;

public class PagoController : Controller
{
    private readonly ILogger<PagoController> _logger;
    private readonly ApplicationDbContext _context;

    public PagoController(ILogger<PagoController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {

        return View();
    }
    public IActionResult Exito()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarPagoConSuscripcion(PagoSuscripcionViewModel model)
    {

        try
        {
            if (string.IsNullOrEmpty(model.Suscripcion.UsuarioId) ||
                model.Suscripcion.CursoId == null ||
                model.Suscripcion.Meses <= 0 ||
                model.Pago.Total <= 0)
            {
                ModelState.AddModelError("", "Datos de suscripción o pago incompletos.");
                return View("Index", model);
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                model.Suscripcion.FechaInicio = DateOnly.FromDateTime(DateTime.Now);
                model.Suscripcion.FechaTermino = DateOnly.FromDateTime(DateTime.Now.AddMonths(model.Suscripcion.Meses));
                _context.Suscripciones.Add(model.Suscripcion);
                await _context.SaveChangesAsync();

                model.Pago.Id = model.Suscripcion.Id;
                model.Pago.NumeroTarjeta =  model.Pago.NumeroTarjeta;              
                _context.Pagos.Add(model.Pago);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }

            return RedirectToAction("Exito"); // Redirect to success page
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error inesperado: " + ex.Message);
            return View("Index", model);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
