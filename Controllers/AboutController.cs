using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProgramaYA.Models;

namespace ProgramaYA.Controllers;

public class AboutController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Acerca de ProgramaYA";
            ViewData["PageDescription"] = "Conoce más sobre ProgramaYA, nuestra misión, visión y valores";
            return View();
        }

        public IActionResult Team()
        {
            ViewData["Title"] = "Nuestro Equipo";
            return View();
        }

        public IActionResult History()
        {
            ViewData["Title"] = "Nuestra Historia";
            return View();
        }
    }