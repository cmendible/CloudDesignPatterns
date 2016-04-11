[assembly: Microsoft.Owin.OwinStartup(typeof(ValetKey.Web.Startup))]
namespace ValetKey.Web
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Owin;
    using System.Net.Http.Formatting;
    using System.Web.Http;

    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            // Add Console Logger
            appBuilder.Use<Logger>();

            // Configure JsonMediaTypeFormatter.
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings =
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

            config.Routes.MapHttpRoute(
                name: "api",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
        }
    }
}
