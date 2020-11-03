using System.Web.Http;
using System.Web.Http.ModelBinding;
using FluentValidation.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Queo.Boards.Infrastructure.ModelBinding;
using Queo.Boards.Infrastructure.Validation;

namespace Queo.Boards {
    /// <summary>
    ///     Web-API-Konfiguration und -Dienste
    /// </summary>
    public static class WebApiConfig {
        public static void Register(HttpConfiguration config) {
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            /* Attribute-Routing aktivieren */
            config.MapHttpAttributeRoutes();

            /* Standard Web-API-Routen */
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            /* ModelBinder konfigurieren */
            config.Services.Insert(typeof(ModelBinderProvider), 0, new EntityModelBinderProvider());

            /* Fluent Validation konfigurieren */
            FluentValidationModelValidatorProvider.Configure(config,
                delegate(FluentValidationModelValidatorProvider provider) {
                    provider.ValidatorFactory = new SpringValidatorFactory();
                });
        }
    }
}