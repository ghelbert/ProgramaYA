using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramaYA.Areas.Identity.Data;
using ProgramaYA.Models;

namespace ProgramaYA.Controllers;

public class NoticiasController : Controller
{
    private readonly ILogger<NoticiasController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly ProgramaYA.Services.INewsApiService _newsApiService;

    public NoticiasController(ILogger<NoticiasController> logger, ApplicationDbContext context, ProgramaYA.Services.INewsApiService newsApiService)
    {
        _logger = logger;
        _context = context;
        _newsApiService = newsApiService;
    }

    public async Task<IActionResult> Index(string q = "bitcoin")
    {
        // Query the NewsAPI for the provided query (default: bitcoin)
        var response = await _newsApiService.GetEverythingAsync(q, pageSize: 9);
        var articles = response?.Articles ?? Enumerable.Empty<ProgramaYA.Models.Dto.Article>();
        return View(articles);
    }
    
    public IActionResult Exito()
    {
        return View();
    }


}
