using System.Net.Http.Formatting;
using System.Web.Http;
using Spring.Web.Mvc;

namespace Queo.Boards {
    public class WebApiApplication : SpringMvcApplication {
        protected void Application_Start() {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            JsonMediaTypeFormatter jsonMediaTypeFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            /*Converter konfigurieren, der Entities anhand der Ids aus einem JSon deserialisieren kann.*/
            jsonMediaTypeFormatter.SerializerSettings.Converters.Add(new JsonEntityConverter());
        }
    }
}