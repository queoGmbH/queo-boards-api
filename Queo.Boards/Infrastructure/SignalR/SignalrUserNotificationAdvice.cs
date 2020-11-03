using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Common.Logging;
using Microsoft.AspNet.SignalR;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Models.Builders;
using Queo.Boards.Hubs;
using Spring.Aop;

namespace Queo.Boards.Infrastructure.SignalR {
    /// <summary>
    ///     Advice, über welches eine SignalR-Benachrichtigung erfolgt, wenn eine <see cref="Notification" /> erstellt wurde.
    /// </summary>
    public class SignalrUserNotificationAdvice : IAfterReturningAdvice {
        /// <summary>
        /// Initialisiert den Advice.
        /// </summary>
        public SignalrUserNotificationAdvice() {
            Debug.WriteLine("SignalrUserNotificationAdvice");
        }

        /// <summary>
        ///     Executes after <paramref name="target" /> <paramref name="method" />
        ///     returns <b>successfully</b>.
        /// </summary>
        /// <remarks>
        ///     <p>
        ///         Note that the supplied <paramref name="returnValue" /> <b>cannot</b>
        ///         be changed by this type of advice... use the around advice type
        ///         (<see cref="T:AopAlliance.Intercept.IMethodInterceptor" />) if you
        ///         need to change the return value of an advised method invocation.
        ///         The data encapsulated by the supplied <paramref name="returnValue" />
        ///         can of course be modified though.
        ///     </p>
        /// </remarks>
        /// <param name="returnValue">
        ///     The value returned by the <paramref name="target" />.
        /// </param>
        /// <param name="method">The intercepted method.</param>
        /// <param name="args">The intercepted method's arguments.</param>
        /// <param name="target">The target object.</param>
        /// <seealso cref="M:AopAlliance.Intercept.IMethodInterceptor.Invoke(AopAlliance.Intercept.IMethodInvocation)" />
        public void AfterReturning(object returnValue, MethodInfo method, object[] args, object target) {
            if (returnValue is IList<Notification>) {
                /*Es wurden mehrere neue Benachrichtigungen erstellt und die Nutzer werden jetzt benachrichtigt*/
                NotifyUsers((IList<Notification>)returnValue);
            } else if (returnValue is Notification) {
                /*Es wurde eine neue Benachrichtigung erstellt und der Nutzer wird jetzt benachrichtigt*/
                NotifyUser((Notification)returnValue);
            } else {
                /*Irgendwas anderes ist im Argen. Auf jeden Fall aber keine Benachrichtigung an den Nutzer.*/
                LogManager.GetLogger("SignalR").InfoFormat("Das {0} informierte über eine neue Benachrichtigung, obwohl der Rückgabewert der aufgerufenen Methode, keine Notification oder Liste von Notifications war.", typeof(SignalrUserNotificationAdvice));
            }
        }

        private void NotifyUser(Notification notification) {
            /*Hub für Nutzerbenachrichtigungen ermitteln.*/
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext(typeof(UserNotificationHub).Name);

            /*Die Benachrichtigung an den Nutzer senden.*/
            NotifyViaHub(hubContext, notification);
        }

        private void NotifyUsers(IList<Notification> notifications) {
            /*Hub für Nutzerbenachrichtigungen ermitteln.*/
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext(typeof(UserNotificationHub).Name);

            foreach (Notification notification in notifications) {
                /*Die Benachrichtigung an den Nutzer senden.*/
                NotifyViaHub(hubContext, notification);
            }
        }

        /// <summary>
        ///     Sendet eine Benachrichtigung über den <see cref="UserNotificationHub" /> an den Nutzer, dass neue
        ///     Benachrichtigungen existieren.
        /// </summary>
        /// <param name="hubContext"></param>
        /// <param name="notification"></param>
        private void NotifyViaHub(IHubContext hubContext, Notification notification) {
            /*Die Gruppe am Hub entspricht dem Channel/Nutzer*/
            hubContext.Clients.Group(notification.NotificationFor.BusinessId.ToString().ToLowerInvariant()).Notification(NotificationModelBuilder.Build(notification));
        }
    }
}