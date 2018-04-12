using System.Web.Http;
using Cqrs.Services;

namespace Les.CSharp.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var directBus = DirectBus.Instance;
            //Domain.Bootstrap.WireUp(directBus, directBus);
        }
    }
}
