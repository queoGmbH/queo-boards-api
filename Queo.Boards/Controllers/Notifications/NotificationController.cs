using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using NSwag.Annotations;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Models.Builders;
using Queo.Boards.Core.Services;
using Queo.Boards.Hubs;
using Queo.Boards.Infrastructure.Controller;
using Queo.Boards.Infrastructure.SignalR;

namespace Queo.Boards.Controllers.Notifications {

    /// <summary>
    /// Controller für die Verarbeitung von Anfragen im Kontext von Benachrichtigungen für Nutzer.
    /// </summary>
    [RoutePrefix("api/notifications")]
    public class NotificationController : AuthorizationRequiredApiController {
        private INotificationService _notificationService;

        public NotificationController(INotificationService notificationService) {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Ruft alle ungelesenen Benachrichtigungen ab.
        /// </summary>
        /// <param name="currentUser">Der Nutzer, dessen ungelesene Benachrichtigungen angezeigt werden sollen.</param>
        /// <param name="read">
        ///     Sollen nur ungelesene Benachrichtigungen abgerufen werden.
        ///     true  = Es werden nur <see cref="Notification.IsMarkedAsRead">gelesene Nachrichten</see> abgerufen.
        ///     false = Es werden nur <see cref="Notification.IsMarkedAsRead">ungelesene Nachrichten</see> abgerufen.
        ///     null  = Es werden alle Nachrichten, unabhängig vom <see cref="Notification.IsMarkedAsRead">Gelesen-Status</see>
        ///     abgerufen.
        /// </param>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IList<NotificationModel>))]
        public IHttpActionResult GetUnreadNotifications([ModelBinder, SwaggerIgnore] User currentUser, [FromUri] bool? read = null) {
            Require.NotNull(currentUser, "currentUser");

            IPage<Notification>foundNotifications = _notificationService.FindForUser(PageRequest.All, currentUser, read);

            return Ok(foundNotifications.Select(NotificationModelBuilder.Build).ToList());
        }

        /// <summary>
        /// Ruft die Anzahl der Benachrichtigungen für einen Nutzer ab.
        /// </summary>
        /// <param name="currentUser">Der Nutzer. für den die Anzahl der Benachrichtigungen abgerufen werden soll.</param>
        /// <param name="read">
        ///     Sollen nur ungelesene Benachrichtigungen abgerufen werden.
        ///     true  = Es werden nur <see cref="Notification.IsMarkedAsRead">gelesene Nachrichten</see> abgerufen.
        ///     false = Es werden nur <see cref="Notification.IsMarkedAsRead">ungelesene Nachrichten</see> abgerufen.
        ///     null  = Es werden alle Nachrichten, unabhängig vom <see cref="Notification.IsMarkedAsRead">Gelesen-Status</see>
        ///     abgerufen.
        /// </param>
        /// <returns></returns>
        [Route("{notification:guid}/count")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(long))]
        public IHttpActionResult GetCount([ModelBinder, SwaggerIgnore] User currentUser, [FromUri] bool? read = null) {
            IPage<Notification> foundNotifications = _notificationService.FindForUser(new PageRequest(1, 1), currentUser, read);
            return Ok(foundNotifications.TotalElements);
        }


        /// <summary>
        /// Aktualisiert den <see cref="Notification.IsMarkedAsRead">Gelesen-Status</see> einer Benachrichtigung.
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="boolValueDto"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        [Route("{notification:guid}/read")]
        [HttpPatch]
        [SwaggerResponse(HttpStatusCode.OK, typeof(NotificationModel))]
        [SwaggerSignalR(typeof(UserNotificationHub), SignalrNotificationScope.CurrentUser)]
        [SignalrNotification(typeof(UserNotificationHub), SignalrNotificationScope.CurrentUser)]
        public IHttpActionResult UpdateIsRead(Notification notification, BoolValueDto boolValueDto, [ModelBinder, SwaggerIgnore] User currentUser) {
            Require.NotNull(notification, "notification");
            Require.NotNull(boolValueDto, "boolValueDto");

            if (!notification.NotificationFor.Equals(currentUser)) {
                return Forbidden("Eine Benachrichtigung kann nur vom Empfänger selbst als gelesen oder ungelesen markiert werden.");
            }

            if (boolValueDto.Value) {
                _notificationService.MarkAsRead(notification, currentUser);
            } else {
                _notificationService.MarkAsUnread(notification, currentUser);
            }

            return Ok(NotificationModelBuilder.Build(notification));
        }

    }
}