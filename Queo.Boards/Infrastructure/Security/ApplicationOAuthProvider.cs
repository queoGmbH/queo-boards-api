using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;

namespace Queo.Boards.Infrastructure.Security {
    /// <summary>
    ///     A provider for the OAuth Authentication for the Web API.
    /// </summary>
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider {
        private readonly string _publicClientId;

        /// <summary>
        ///     Creates a new instance for <see cref="ApplicationOAuthProvider" />.
        /// </summary>
        /// <param name="publicClientId"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApplicationOAuthProvider(string publicClientId) {
            if (publicClientId == null) {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        /// <summary>
        ///     Creates the <see cref="AuthenticationProperties" /> for a given username.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static AuthenticationProperties CreateProperties(string userName) {
            IDictionary<string, string> data = new Dictionary<string, string> {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }

        /// <inheritdoc />
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context) {
            ApplicationUserManager userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            SecurityUser user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null) {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            if (!user.IsEnabled) {
                context.SetError("invalid_grant", "The user is disabled.");
                return;
            }

            ClaimsIdentity oAuthIdentity = await userManager.CreateIdentityAsync(user, OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookieIdentity = await userManager.CreateIdentityAsync(user, CookieAuthenticationDefaults.AuthenticationType);

            AuthenticationProperties properties = CreateProperties(user.UserName);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookieIdentity);
        }

        /// <inheritdoc />
        public override Task TokenEndpoint(OAuthTokenEndpointContext context) {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary) {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        /// <inheritdoc />
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context) {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null) {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        /// <inheritdoc />
        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context) {
            if (context.ClientId == _publicClientId) {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri) {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }
    }
}