using System.Web.Http;
using TorneosITM_API.Clases; // Namespace correcto para TokenValidationHandler

namespace TorneosITM_API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            // --- ACTIVAR EL HANDLER DE VALIDACIÓN ---
            config.MessageHandlers.Add(new TokenValidationHandler());
            // ----------------------------------------

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var formatters = config.Formatters;
            formatters.Remove(formatters.XmlFormatter);
        }
    }
}