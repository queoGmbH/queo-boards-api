using System;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Queo.Boards.Infrastructure.Security;

namespace Queo.Boards {
    

    /// <summary>
    /// Teil der Startup-Klasse, der für die Konfiguration der Authentifizierung verantwortlich ist.
    /// </summary>
    public partial class Startup {

        /// <summary>
        ///     Gets the public client id
        /// </summary>
        public static string PublicClientId {
            get; private set;
        }

        /// <summary>
        ///     Gets the oauth options
        /// </summary>
        public static OAuthAuthorizationServerOptions OAuthOptions {
            get; private set;
        }

        public void ConfigureAuthentication(IAppBuilder app) {


            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            app.UseCors(CorsOptions.AllowAll);

            // need to add UserManager into owin, because this is used in cookie invalidation
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(12),
                
                //AllowInsecureHttp = true
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            HttpConfiguration config = new HttpConfiguration();
            app.UseWebApi(config);
        }
    }
}