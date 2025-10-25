using Microsoft.AspNetCore.Mvc;
using ProgramaYA.Models.Dto;
using ProgramaYA.Services;

namespace ProgramaYA.Controllers;

public class WeatherController : Controller
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    public async Task<IActionResult> Index(decimal? lat, decimal? lon, string units = "imperial", string lang = "es")
    {
        var vm = new WeatherViewModel();
        vm.Lat = lat ?? vm.Lat;
        vm.Lon = lon ?? vm.Lon;
        vm.Units = units;
        vm.Lang = lang;

        var (success, data, raw, error) = await _weatherService.GetCurrentAsync(vm.Lat, vm.Lon, vm.Units, vm.Lang);
        if (success)
        {
            vm.Weather = data;
            vm.RawJson = raw;
        }
        else
        {
            vm.Error = error ?? "Error desconocido";
            vm.RawJson = raw;
        }

        return View(vm);
    }
}
