using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UrlShrinker.Startup))]
namespace UrlShrinker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {


        }
    }
}
