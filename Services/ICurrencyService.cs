using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProgramaYA.Services
{
    public interface ICurrencyService
    {
        Task<Dictionary<string, string>> GetCurrenciesAsync();
        Task<(bool Success, decimal? Result, string RawMessage)> ConvertAsync(string from, string to, decimal amount);
    }
}
