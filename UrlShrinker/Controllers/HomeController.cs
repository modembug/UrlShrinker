using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using UrlShrinker.Business;
using UrlShrinker.Models;

namespace UrlShrinker.Controllers
{
    public class HomeController : Controller
    {
        private IEntryService _entryService;
        private IValidationService _validationService;

        public HomeController()
        {
            _entryService = new EntryService();
            _validationService = new ValidationService();
        }

        [HttpGet]
        public async Task<ActionResult> Index(string token)
        {
            //Determine if a token is in the URL.
            if (string.IsNullOrEmpty(token))
            {
                return View(new HomeModel());
            }
            try
            {
                return Redirect(await _entryService.GetEntry(new HomeModel() { Token = token }, QueryType.Url));
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Home", "Index"));
            }
        }

        private async Task ProcessValidationErrors(IValidationContext context)
        {
            await Task.Run(() =>
            {
                if (!context.IsValid)
                {
                    foreach (string error in context.ErrorMessages)
                    {
                        ModelState.AddModelError("url", error);
                    }
                }
            });
        }

        public async Task<ActionResult> Index(HomeModel model)
        {
            model.Host = Request.Url.Host;

            if (Request.Form.GetKey(1) == "shrink")
            {
                IValidationContext context = await _validationService.UrlValid(model.Url);

                await ProcessValidationErrors(context);

                if (ModelState.IsValid)
                {
                    model.Host = Request.Url.Host;
                    model.Url = await _entryService.AddEntry(model);
                }
            }

            if (Request.Form.GetKey(1) == "inflate")
            {
                IValidationContext context = await _validationService.ShortUrlValid(model);

                await ProcessValidationErrors(context);

                try
                {
                    model.Token = Regex.Replace(model.Url, String.Format("(http://)({0})(/)(\\w*)", Request.Url.Host),
                        "$4");
                    model.Url = await _entryService.GetEntry(model, QueryType.Url);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("url", e.Message);
                }

                return View(model);
            }

            return View(model);
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult ApiRef()
        {
            return View();
        }
    }
}