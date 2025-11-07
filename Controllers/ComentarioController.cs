using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using apptienda.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// using apptienda.ML;
using ProgramaYA.Areas.Identity.Data;
using ProgramaYA.ML.ModelSentimental;
using ProgramaYA.Models;

namespace apptienda.Controllers
{
    public class ComentarioController : Controller
    {
        private readonly ILogger<ComentarioController> _logger;
        private readonly ApplicationDbContext _context;


        public ComentarioController(ILogger<ComentarioController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<CursosList> lista = await _context.Cursos
            .Select(s => new CursosList
            {
                Id = s.Id,
                Nombre = s.Nombre
            })
            .ToListAsync();
            ComentarioCursos comentarios = new();
            comentarios.Cursos = lista;
            return View(comentarios);
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(Comentario comentario)
        {
            List<CursosList> lista = await _context.Cursos
            .Select(s => new CursosList
            {
                Id = s.Id,
                Nombre = s.Nombre
            })
            .ToListAsync();
            ComentarioCursos comentarios = new();
            comentarios.Cursos = lista;
            if (ModelState.IsValid)
            {
                try
                {

                    //Load sample data
                    var sampleData = new MLModel1.ModelInput()
                    {
                        Comentario = comentario.Mensaje
                    };

                    //Load model and predict output
                    var result = MLModel1.Predict(sampleData);
                    var predictedLabel = result.PredictedLabel;
                    var scorePositive = result.Score[0];
                    var scoreNegative = result.Score[1];
                    //Check if the result is positive or negative
                    if (predictedLabel == 1)
                    {
                        comentario.Etiqueta = "Positivo";
                        comentario.Puntuacion = scorePositive;
                    }
                    else
                    {
                        comentario.Etiqueta = "Negativo";
                        comentario.Puntuacion = scoreNegative;
                    }


                    _context.Comentarios.Add(comentario);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Se registró la reseña");
                    ViewData["Message"] = "Se registró la reseña";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al registrar la reseña");
                    ViewData["Message"] = "Error al registrar la reseña: " + ex.Message;
                }
            }
            else
            {
                ViewData["Message"] = "Datos de entrada no válidos";
            }
            return View("Index", comentarios);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}