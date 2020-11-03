using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Common.Logging;
using Microsoft.AspNet.Identity;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Infrastructure.Http;

namespace Queo.Boards.Infrastructure.Filter {

    /// <summary>
    ///     Filter, der steuert, ob der Request im Zusammenhang mit einer Benachrichtigung erfolgt, auf welche der angemeldete Nutzer zugreifen kann.
    ///
    ///     Dazu wird die URL nach der Angabe einer Benachrichtigung durchsucht und für die Benachrichtigung kontrolliert, ob der Nutzer darauf
    ///     zugreifen kann.
    /// </summary>
    public class NotificationScopeAuthorizationFilterAttribute : ActionFilterAttribute {
        private const string LOGGER_NAME = "Security";



        /// <summary>Tritt vor dem Aufrufen der Aktionsmethode auf.</summary>
        /// <param name="actionContext">Der Aktionskontext.</param>
        public override void OnActionExecuting(HttpActionContext actionContext) {
            ILog logger = LogManager.GetLogger(LOGGER_NAME);

            Notification notification;
            if (HttpActionContextHelper.TryGetNotificationScopeFromAction(actionContext, out notification)) {

                if (!AuthorizeViewNotification(notification, actionContext.RequestContext.Principal)) {
                    logger.InfoFormat("Der Nutzer [{0}] hat keine Berechtigung, die Benachrichtigung [{1} => {2}] zu sehen.", actionContext.RequestContext.Principal.Identity.Name, notification.Id, notification.DisplayText);
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
                    return;
                }

                if (actionContext.Request.Method != HttpMethod.Get) {
                    /*Wenn es sich nicht um ein Get handelt, dann auf Recht zum Ändern prüfen.*/
                    if (!AuthorizeUpdateNotification(notification, actionContext.RequestContext.Principal)) {
                        logger.InfoFormat("Der Nutzer [{0}] hat keine Berechtigung, die Benachrichtigung [{1} => {2}] zu bearbeiten.", actionContext.RequestContext.Principal.Identity.Name, notification.Id, notification.DisplayText);
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
                        return;
                    }
                }


            }

        }

        private bool AuthorizeUpdateNotification(Notification notification, IPrincipal requestContextPrincipal) {
            Require.NotNull(notification, "notification");
            Require.NotNull(requestContextPrincipal, "requestContextPrincipal");
            ILog logger = LogManager.GetLogger(LOGGER_NAME);

            if (notification.NotificationFor.BusinessId.ToString() == requestContextPrincipal.Identity.GetUserId()) {
                logger.DebugFormat("Der Nutzer [{0}] darf die Benachrichtigung sehen [{1} => {2}] sehen, da er der Empfänger ist.", requestContextPrincipal.Identity.Name, notification.Id, notification.DisplayText);
                return true;
            } else {
                logger.DebugFormat("Der Nutzer [{0}] darf die Benachrichtigung NICHT sehen [{1} => {2}] sehen, da er nicht der Empfänger ist.", requestContextPrincipal.Identity.Name, notification.Id, notification.DisplayText);
                return false;
            }
        }

        private bool AuthorizeViewNotification(Notification notification, IPrincipal requestContextPrincipal) {
            Require.NotNull(notification, "notification");
            Require.NotNull(requestContextPrincipal, "requestContextPrincipal");
            ILog logger = LogManager.GetLogger(LOGGER_NAME);

            if (notification.NotificationFor.BusinessId.ToString() == requestContextPrincipal.Identity.GetUserId()) {
                logger.DebugFormat("Der Nutzer [{0}] darf die Benachrichtigung bearbeiten [{1} => {2}] sehen, da er der Empfänger ist.", requestContextPrincipal.Identity.Name, notification.Id, notification.DisplayText);
                return true;
            } else {
                logger.DebugFormat("Der Nutzer [{0}] darf die Benachrichtigung NICHT bearbeiten [{1} => {2}] sehen, da er nicht der Empfänger ist.", requestContextPrincipal.Identity.Name, notification.Id, notification.DisplayText);
                return false;
            }
        }
    }
}