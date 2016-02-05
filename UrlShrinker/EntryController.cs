using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using UrlShrinker.Business;
using UrlShrinker.Models;

namespace UrlShrinker
{
    public class EntryController : ApiController
    {
        private IEntryService _entryService;
        private IValidationService _validationService;

        public EntryController()
        {
            _entryService = new EntryService();
            _validationService = new ValidationService();
        }

        // GET: api/Entry/Token
        public async Task<HttpResponseMessage> Get(string id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.Accepted, 
                    await _entryService.GetEntry(new HomeModel() {Token = id}, QueryType.Url));
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            
        }

        // POST: api/Entry/URL
        public async Task<HttpResponseMessage> Post(HttpRequestMessage request) 
        {
            string data = await request.Content.ReadAsStringAsync();

            string url = string.Empty;

            if (!string.IsNullOrEmpty(data))
            {
                url = HttpUtility.HtmlEncode(data.Trim());
            }

            IValidationContext context = await _validationService.UrlValid(url);

            if (!context.IsValid)
            {
                StringBuilder sb = new StringBuilder();

                foreach (string error in context.ErrorMessages)
                {
                    sb.Append(error).Append(" ");
                }

                return request.CreateResponse(HttpStatusCode.BadRequest, sb.ToString().Trim());
            }

            return request.CreateResponse(HttpStatusCode.Accepted,
                await
                    _entryService.AddEntry(new HomeModel()
                    {
                        Url = HttpUtility.HtmlEncode(url),
                        Host = request.RequestUri.Host
                    }));
        }
    }

}
