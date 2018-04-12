using System.Web.Http;
using Newtonsoft.Json.Converters;

namespace Les.CSharp.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.EnableCors();
            
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "API Default",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: new { id = @"^|[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}$" }
            );

            config.Routes.MapHttpRoute(
                name: "ApiWithAction",
                routeTemplate: "{controller}/{action}"
            );

            config.Formatters.JsonFormatter.SerializerSettings.Converters
                .Add(new StringEnumConverter());
        }
    }
}
