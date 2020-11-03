using System.Collections.Generic;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Core.Services {
    /// <summary>
    ///     Schnittstelle, die einen Service zur Verwaltung von Benachrichtigungen beschreibt.
    /// </summary>
    public interface INotificationService {


        /// <summary>
        /// Erstellt die Benachrichtigungen über eine Aktion mit einer Karte, für eine Reihe von Nutzern.
        /// </summary>
        /// <param name="users">Die Nutzer, welche die Benachrichtigung erhalten sollen.</param>
        /// <param name="card">Die Karte, mit oder an der die Aktion durchgeführt wurde.</param>
        /// <param name="reason">Der Anlass, warum es eine Benachrichtigung gibt.</param>
        /// <returns></returns>
        IList<CardNotification> CreateCardNotification(IList<User> users, Card card, CardNotificationReason reason);

        /// <summary>
        /// Erstellt die Benachrichtigungen über eine Aktion mit einem Kommentar, für eine Reihe von Nutzern.
        /// </summary>
        /// <param name="users">Die Nutzer, welche die Benachrichtigung erhalten sollen.</param>
        /// <param name="comment">Der Kommentar, mit oder an dem die Aktion durchgeführt wurde.</param>
        /// <param name="reason">Der Anlass, warum es eine Benachrichtigung gibt.</param>
        /// <returns></returns>
        IList<CommentNotification> CreateCommentNotification(IList<User> users, Comment comment, CommentNotificationReason reason);


        /// <summary>
        /// Erstellt Benachrichtigungen für die an einem Board angemeldeten Nutzer, wenn das Fälligkeitsdatum einer Karte abgelaufen ist.
        /// </summary>
        /// <returns></returns>
        IList<CardNotification> CreateCardNotificationsForDueExpiration();

        /// <summary>
        /// Versendet für alle nicht gelesenen <see cref="Notification">Benachrichtigungen</see> eine E-Mail.
        /// </summary>
        /// <returns>Liste mit den Benachrichtigungen, für die eine E-Mail versendet wurde.</returns>
        IList<Notification> SendEmailForUnreadNotifications();

        /// <summary>
        /// Ruft seitenweise alle Benachrichtigungen für den Nutzer ab.
        /// </summary>
        /// <param name="pageRequest">Seiteninformationen</param>
        /// <param name="user">Der Nutzer, dessen Benachrichtigungen abgerufen werden sollen.</param>
        /// <param name="isMarkedAsRead">
        ///     Sollen nur ungelesene Benachrichtigungen abgerufen werden.
        ///     true  = Es werden nur <see cref="Notification.IsMarkedAsRead">gelesene Nachrichten</see> abgerufen.
        ///     false = Es werden nur <see cref="Notification.IsMarkedAsRead">ungelesene Nachrichten</see> abgerufen.
        ///     null  = Es werden alle Nachrichten, unabhängig vom <see cref="Notification.IsMarkedAsRead">Gelesen-Status</see>
        ///     abgerufen.
        /// </param>
        /// <returns></returns>
        IPage<Notification> FindForUser(IPageable pageRequest, User user, bool? isMarkedAsRead);

        /// <summary>
        /// Markiert eine Benachrichtigung als gelesen.
        /// Ist die Nachricht bereits als gelesen markiert, werden keine Änderungen vorgenommen.
        /// </summary>
        /// <param name="notification">Die Nachricht, die als gelesen markiert werden soll.</param>
        /// <param name="user">Der Nutzer, der die Nachricht als gelesen markiert.</param>
        void MarkAsRead(Notification notification, User user);

        /// <summary>
        /// Markiert eine Benachrichtigung als ungelesen.
        /// Ist die Nachricht bereits als ungelesen markiert, werden keine Änderungen vorgenommen.
        /// </summary>
        /// <param name="notification">Die Nachricht, die als ungelesen markiert werden soll.</param>
        /// <param name="user">Der Nutzer, der die Nachricht als ungelesen markiert.</param>
        void MarkAsUnread(Notification notification, User user);
    }
}