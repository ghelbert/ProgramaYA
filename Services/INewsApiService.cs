using System.Threading.Tasks;
using ProgramaYA.Models.Dto;

namespace ProgramaYA.Services
{
    public interface INewsApiService
    {
        Task<NewsApiResponse> GetEverythingAsync(string query, int pageSize = 20);
    }
}
