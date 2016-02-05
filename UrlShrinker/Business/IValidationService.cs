
using System.Threading.Tasks;
using UrlShrinker.Models;

namespace UrlShrinker.Business
{
    public interface IValidationService
    {
        Task<IValidationContext> UrlValid(string url);

        Task<IValidationContext> ShortUrlValid(IHomeModel model);
    }
}
