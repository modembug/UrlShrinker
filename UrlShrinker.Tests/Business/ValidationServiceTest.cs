using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlShrinker.Business;
using UrlShrinker.Models;

namespace UrlShrinker.Tests.Business
{
    [TestClass]
    public class ValidationServiceTest
    {
        [TestMethod]
        public async Task UrlValid()
        {
            IValidationService service = new ValidationService();

            List<string> validUrlList = new List<string>();

            validUrlList.Add("http://google.com");
            validUrlList.Add("http://www.bing.com/search?q=skdjfs%20sdlkfjdsf%20sldkfjdsklfjsdfsj89ser78w36r%209Ees7R*F(SEF%5E*(6sdd7fs798f6(Df)&qs=n&form=QBRE&pq=skdjfs%20sdlkfjdsf%20sldkfjdsklfjsdfsj89ser78w36r%209ees7r*f(sef%5E*(6sdd7fs798f6(df)&sc=0-7&sp=-1&sk=&cvid=D258D77CEE4648F2BB4A5AF45E284483");
            validUrlList.Add("https://www.google.com/#q=sadfkjdshf+skjddsfhHF*R(%5EE%26r7803467*)ER2306+r58302837+7tetr8i()%40)7r38436%25%24%23%24%24%235oefrp9es8r7e09");

            foreach (string url in validUrlList)
            {
                IValidationContext context = await service.UrlValid(url);

                Assert.IsTrue(context.IsValid);
                Assert.IsTrue(context.ErrorMessages.Count == 0);
            }
        }

        [TestMethod]
        public async Task UrlInvalid()
        {
            IValidationService service = new ValidationService();

            List<string> invalidUrlList = new List<string>();

            invalidUrlList.Add("");
            invalidUrlList.Add("google.com");
            invalidUrlList.Add("http://google.com{");
            invalidUrlList.Add("<SCRIPT SRC=http://ha.ckers.org/xss.js></SCRIPT>");
            invalidUrlList.Add("<IMG SRC=javascript:alert('XSS')>");
            invalidUrlList.Add("<IMG SRC=JaVaScRiPt:alert('XSS')>");
            invalidUrlList.Add("<IMG SRC=javascript:alert(String.fromCharCode(88,83,83))>");
            invalidUrlList.Add("http://google.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");



            foreach (string url in invalidUrlList)
            {
                IValidationContext context = await service.UrlValid(url);

                Assert.IsTrue(!context.IsValid);
                Assert.IsTrue(context.ErrorMessages.Count != 0);
            }
        }

        [TestMethod]
        public async Task ShortUrlValid()
        {
            IValidationService service = new ValidationService();

            List<IHomeModel> validUrlList = new List<IHomeModel>();

            validUrlList.Add(new HomeModel() { Host = "www.shrinkurl.com", Url = "http://www.shrinkurl.com/a1b2c3" });
            validUrlList.Add(new HomeModel() { Host = "localhost", Url = "http://localhost/a1b2c3" });

            foreach (IHomeModel model in validUrlList)
            {

                IValidationContext context = await service.ShortUrlValid(model);

                Assert.IsTrue(context.IsValid);
                Assert.IsTrue(context.ErrorMessages.Count == 0);
            }
        }

        [TestMethod]
        public async Task ShortUrlInvalid()
        {
            IValidationService service = new ValidationService();

            List<IHomeModel> validUrlList = new List<IHomeModel>();

            validUrlList.Add(new HomeModel() { Host = "www.shrinkurl.com", Url = "http://www.shrinkurl.com/a1b2c3{" });
            validUrlList.Add(new HomeModel() { Host = "localhost", Url = "http://localhost/a$" });
            validUrlList.Add(new HomeModel() { Host = "localhost_badHostname", Url = "http://localhost/" });
            validUrlList.Add(new HomeModel() { Host = "localhost", Url = "localhost/111" });


            foreach (IHomeModel model in validUrlList)
            {

                IValidationContext context = await service.ShortUrlValid(model);

                Assert.IsTrue(!context.IsValid);
                Assert.IsTrue(context.ErrorMessages.Count != 0);
            }
        }
    }
}
