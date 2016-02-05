
using System.Threading.Tasks;

namespace UrlShrinker.Business
{
    public interface ITokenService
    {
        Task<string> GetToken();
    }
}
