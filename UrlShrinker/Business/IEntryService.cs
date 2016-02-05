using System.Threading.Tasks;
using UrlShrinker.Models;

namespace UrlShrinker.Business
{
    public interface IEntryService
    {
        Task<string> AddEntry(IHomeModel model);

        Task DeleteEntry(IHomeModel model, QueryType queryType);

        Task<string> GetEntry(IHomeModel model, QueryType queryType);

        Task<bool> Exists(IHomeModel model, QueryType queryType);

        Task CreateEntryStorage();
    }
}
