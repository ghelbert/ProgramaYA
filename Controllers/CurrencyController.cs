using Microsoft.AspNetCore.Mvc;
using ProgramaYA.Models.Dto;
using ProgramaYA.Services;

namespace ProgramaYA.Controllers;

public class CurrencyController : Controller
{
    private readonly ICurrencyService _currencyService;
    private readonly ILogger<CurrencyController> _logger;

    public CurrencyController(ICurrencyService currencyService, ILogger<CurrencyController> logger)
    {
        _currencyService = currencyService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string from = "USD", string to = "EUR", decimal amount = 1)
    {
        var vm = new CurrencyViewModel
        {
            From = from,
            To = to,
            Amount = amount
        };

        // Load currencies list
        vm.Currencies = await _currencyService.GetCurrenciesAsync();

        // If query parameters provided, run conversion
        if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to) && amount > 0)
        {
            var (success, result, raw) = await _currencyService.ConvertAsync(from, to, amount);
            if (success)
            {
                vm.Result = result;
            }
            else
            {
                vm.ErrorMessage = "La conversión falló. Revisa logs para más detalles.";
                vm.RawApiMessage = raw;
            }
        }

        return View(vm);
    }
}
