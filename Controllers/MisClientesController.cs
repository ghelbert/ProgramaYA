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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}