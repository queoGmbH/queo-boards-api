using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;

// ReSharper disable once CheckNamespace
// Der Namespace muss der Default-Namespace der Anwendung sein, damit dieses Attribute zuerst verwendet wird.
namespace Queo.Boards.Infrastructure.Http {
    /// <summary>
    ///     "Überladung" des originalen AuthorizeAttributes, damit nicht immer eine 401 (Unauthorized) als Fehlercode geliefert
    ///     wird.
    ///     https://stackoverflow.com/questions/238437/why-does-authorizeattribute-redirect-to-the-login-page-for-authentication-and-au
    ///     Wenn der Request zwar Authentifiziert ist und nur die Autorisierung fehlt, soll besser eine 403 (Forbidden)
    ///     geliefert werden.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeAttribute : System.Web.Http.AuthorizeAttribute {
        /// <summary>Verarbeitet Anforderungen, deren Autorisierung nicht erfolgreich war.</summary>
        /// <param name="actionContext">Der Kontext.</param>
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext) {
            if (actionContext.RequestContext.Principal.Identity.IsAuthenticated) {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
            } else {
                base.HandleUnauthorizedRequest(actionContext);
            }
        }
    }
}