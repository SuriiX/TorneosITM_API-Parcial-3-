using Microsoft.Owin;
using Owin;
using System.Web.Http;

[assembly: OwinStartup(typeof(TorneosITM_API.Clases.Startup))]


namespace TorneosITM_API.Clases 
{
    public class Startup 
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseWebApi(config);
        }
    }
}