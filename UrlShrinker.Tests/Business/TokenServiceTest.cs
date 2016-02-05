using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlShrinker.Business;

namespace UrlShrinker.Tests.Business
{
    [TestClass]
    public class TokenTest
    {

        [TestMethod]
        public async Task GetRandomCharactersNotNull()
        {
            ITokenService tokenService = new TokenService();
            Assert.IsNotNull(await tokenService.GetToken());
        }

        [TestMethod]
        public async Task GetRandomCharactersNotEmpty()
        {
            ITokenService tokenService = new TokenService();
            Assert.AreNotEqual(await tokenService.GetToken(), string.Empty);
        }

    }
}
