using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace Queo.Boards {
    /// <summary>
    ///     Teil der Startup-Klasse, der für die Konfiguration von SignalR verantwortlich ist.
    /// </summary>
    public partial class Startup {
        private static void ConfigureSignalR(IAppBuilder app, IDependencyResolver dependencyResolver) {
            /* SignalR */

            app.Map(
                "/signalr",
                map => {
                    map.UseCors(CorsOptions.AllowAll);
                    map.UseOAuthBearerAuthentication(
                        new OAuthBearerAuthenticationOptions {
                            Provider = new QueryStringOAuthBearerProvider()
                        });

                    HubConfiguration hubConfiguration = new HubConfiguration();
                    hubConfiguration.EnableDetailedErrors = true;
                    hubConfiguration.EnableJSONP = true;

                    hubConfiguration.Resolver = dependencyResolver;
                    map.RunSignalR(hubConfiguration);
                });

            GlobalHost.DependencyResolver = dependencyResolver;
            //GlobalConfiguration.Configuration.Filters.Add(new SignalrNotificationAttribute());
            GlobalHost.HubPipeline.RequireAuthentication();
        }
    }
}