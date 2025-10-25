using System.Threading.Tasks;
using ProgramaYA.Models.Dto;

namespace ProgramaYA.Services
{
    public interface IWeatherService
    {
        Task<(bool Success, WeatherData Data, string Raw, string Error)> GetCurrentAsync(decimal lat, decimal lon, string units = "metric", string lang = "es");
    }
}
