using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using UrlShrinker.Business;

namespace UrlShrinker
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private EntryService _entryService;

        public MvcApplication()
        {
            _entryService = new EntryService();
        }

        protected void Application_Start()
        {
            Task.Run(async () => await _entryService.CreateEntryStorage());

            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
