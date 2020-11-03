using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;

namespace Queo.Boards {
    /// <summary>
    /// Provider zum Auslesen des OAuth-Tokens aus dem Query String beim Verbinden mit einem SignalR-Hub.
    /// </summary>
    public class QueryStringOAuthBearerProvider : OAuthBearerAuthenticationProvider {
        public override Task RequestToken(OAuthRequestTokenContext context) {
            string value = context.Request.Query.Get("auth_token");

            if (!string.IsNullOrEmpty(value)) {
                context.Token = value;
            }

            return Task.FromResult<object>(null);
        }
    }
}