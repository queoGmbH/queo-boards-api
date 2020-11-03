using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Persistence.Impl;
using Spring.Transaction.Interceptor;

namespace Queo.Boards.Core.Services.Impl {
    /// <summary>
    ///     Service, der Methoden zur Verwaltung von Benachrichtigungen bereitstellt.
    /// </summary>
    public class NotificationService : INotificationService {
        /// <summary>
        ///     Ruft den Vorlauf in Minuten ab, mit dem die Fälligkeit von Karten ermittelt wird.
        ///     Bei einem Wert von 60, erfolgt 60 Minuten vor dem tatsächlichen Fälligkeitsdatum der Karte die Benachrichtigung.
        /// </summary>
        public const int CARDS_DUE_LIMIT_OFFSET_IN_MINUTES = 60;

        private readonly ICardDao _cardDao;

        private readonly INotificationDao _notificationDao;

        private readonly IEmailNotificationService _emailNotificationService;

        public NotificationService(INotificationDao notificationDao, ICardDao cardDao, IEmailNotificationService emailNotificationService) {
            _notificationDao = notificationDao;
            _cardDao = cardDao;
            _emailNotificationService = emailNotificationService;
        }

        /// <summary>
        ///     Ermittelt die aktuelle Grenze für die Benachrichtigung über den Ablauf des Fälligkeitsdatums einer Karte ab.
        /// </summary>
        /// <returns></returns>
        public static DateTime GetCurrentDueLimit() {
            /*"Die Benachrichtigung erfolgt 1h vor Ablauf der Karte"*/
            return DateTime.UtcNow.AddMinutes(CARDS_DUE_LIMIT_OFFSET_IN_MINUTES);
        }

        /// <summary>
        ///     Erstellt die Benachrichtigungen über eine Aktion mit einer Karte, für eine Reihe von Nutzern.
        /// </summary>
        /// <param name="users">Die Nutzer, welche die Benachrichtigung erhalten sollen.</param>
        /// <param name="card">Die Karte, mit oder an der die Aktion durchgeführt wurde.</param>
        /// <param name="reason">Der Anlass, warum es eine Benachrichtigung gibt.</param>
        /// <returns></returns>
        [Transaction]
        public IList<CardNotification> CreateCardNotification(IList<User> users, Card card, CardNotificationReason reason) {
            Require.NotNull(card, "card");
            Require.NotNull(users, "users");

            IList<CardNotification> notifications = users.Select(user => new CardNotification(card, reason, user, DateTime.Now)).ToList();
            foreach (CardNotification cardNotification in notifications) {
                card.CardNotifications.Add(cardNotification);
            }
            card.UpdateDueExpirationNotificationCreated(DateTime.Now);
            _notificationDao.Save(notifications.ToList<Notification>());

            return notifications;
        }

        /// <summary>
        ///     Erstellt Benachrichtigungen für die an einem Board angemeldeten Nutzer, wenn das Fälligkeitsdatum einer Karte
        ///     abgelaufen ist.
        /// </summary>
        /// <returns></returns>
        [Transaction]
        public IList<CardNotification> CreateCardNotificationsForDueExpiration() {
            /*Aktuelle Grenze für die Fälligkeits-Benachrichtigung ermitteln.*/
            DateTime dueExpirationLimit = GetCurrentDueLimit();

            /*Karten ermitteln, für die eine Benachrichtigung erfolgen muss.*/
            IList<Card> expiredCardsWithoutNotifications = _cardDao.FindCardsWithExpiredDueAndWithoutNotifications(dueExpirationLimit);

            /*Benachrichtigung(en) erstellen*/
            List<CardNotification> createdNotifications = new List<CardNotification>();
            foreach (Card expiredCard in expiredCardsWithoutNotifications) {
                IList<User> usersToNotify = expiredCard.AssignedUsers.ToList();
                if (!usersToNotify.Contains(expiredCard.CreatedBy)) {
                    /*Wenn nicht schon gemacht, dann den Ersteller der Karte ebenfalls benachrichtigen*/
                    usersToNotify.Add(expiredCard.CreatedBy);
                }

                createdNotifications.AddRange(CreateCardNotification(usersToNotify, expiredCard, CardNotificationReason.DueExpiration));
            }

            return createdNotifications;
        }

        /// <summary>
        ///     Erstellt die Benachrichtigungen über eine Aktion mit einem Kommentar, für eine Reihe von Nutzern.
        /// </summary>
        /// <param name="users">Die Nutzer, welche die Benachrichtigung erhalten sollen.</param>
        /// <param name="comment">Der Kommentar, mit oder an dem die Aktion durchgeführt wurde.</param>
        /// <param name="reason">Der Anlass, warum es eine Benachrichtigung gibt.</param>
        /// <returns></returns>
        [Transaction]
        public IList<CommentNotification> CreateCommentNotification(IList<User> users, Comment comment, CommentNotificationReason reason) {
            Require.NotNull(users, "users");
            Require.NotNull(comment, "comment");

            IList<CommentNotification> notifications = users.Select(user => new CommentNotification(comment, reason, user, DateTime.Now)).ToList();
            foreach (CommentNotification commentNotification in notifications) {
                comment.CommentNotifications.Add(commentNotification);
            }
            _notificationDao.Save(notifications.ToList<Notification>());

            return notifications;
        }

        /// <summary>
        ///     Versendet für alle nicht gelesenen <see cref="Notification">Benachrichtigungen</see> eine E-Mail.
        /// </summary>
        /// <returns>Liste mit den Benachrichtigungen, für die eine E-Mail versendet wurde.</returns>
        [Transaction]
        public IList<Notification> SendEmailForUnreadNotifications() {

            LogManager.GetLogger("Notifications").DebugFormat("Batch-Lauf für des Versenden von E-Mails über nicht gelesene Benachrichtigungen.");

            /*Die Benachrichtigungen laden, für die eine E-Mail versendet werden muss.*/
            IList<Notification> sendEmailForNofications = _notificationDao.FindNotificationWhereToSendEmail();

            /*E-Mails versenden*/
            foreach (Notification notificationToSendEmailFor in sendEmailForNofications.Where(notification => !string.IsNullOrWhiteSpace(notification.NotificationFor.Email))) {
                switch (notificationToSendEmailFor.NotificationCategory) {
                    case NotificationCategory.Card:
                        EmailOnCardNotification(notificationToSendEmailFor as CardNotification);
                        break;
                    case NotificationCategory.Comment:
                        EmailOnCommentNotification(notificationToSendEmailFor as CommentNotification);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                notificationToSendEmailFor.MarkAsEmailSend(DateTime.UtcNow);
                LogManager.GetLogger("Notifications").DebugFormat("Es wurde eine E-Mail für die Benachrichtigung {0} an {1} gesendet.", notificationToSendEmailFor.DisplayText, notificationToSendEmailFor.NotificationFor);
            }

            LogManager.GetLogger("Notifications").InfoFormat("Es wurden E-Mails für {0} Benachrichtigung(en) versendet.", sendEmailForNofications.Count);

            return sendEmailForNofications;
        }

        /// <summary>
        ///     Versendet eine E-Mail für die Benachrichtigung über Karten.
        /// </summary>
        private void EmailOnCardNotification(CardNotification cardNotification) {
            switch (cardNotification.NotificationReason) {
                case CardNotificationReason.DueExpiration:
                    _emailNotificationService.NotifyUserOnCardDueExpiration(cardNotification.NotificationFor, cardNotification.Card);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Versendet eine E-Mail für die Benachrichtigung über Kommentare.
        /// </summary>
        /// <param name="commentNotification"></param>
        private void EmailOnCommentNotification(CommentNotification commentNotification) {
            switch (commentNotification.NotificationReason) {
                case CommentNotificationReason.CommentCreated:
                    _emailNotificationService.NotifyUserOnCommentCreated(commentNotification.NotificationFor, commentNotification.Comment);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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
        public IPage<Notification> FindForUser(IPageable pageRequest, User user, bool? isMarkedAsRead) {
            return _notificationDao.FindForUser(pageRequest, user, isMarkedAsRead);
        }

        /// <summary>
        /// Markiert eine Benachrichtigung als gelesen.
        /// Ist die Nachricht bereits als gelesen markiert, werden keine Änderungen vorgenommen.
        /// </summary>
        /// <param name="notification">Die Nachricht, die als gelesen markiert werden soll.</param>
        /// <param name="user">Der Nutzer, der die Nachricht als gelesen markiert.</param>
        [Transaction]
        public void MarkAsRead(Notification notification, User user) {
            Require.NotNull(notification, "notification");
            Require.NotNull(user, "user");

            notification.MarkAsRead();
        }

        /// <summary>
        /// Markiert eine Benachrichtigung als ungelesen.
        /// Ist die Nachricht bereits als ungelesen markiert, werden keine Änderungen vorgenommen.
        /// </summary>
        /// <param name="notification">Die Nachricht, die als ungelesen markiert werden soll.</param>
        /// <param name="user">Der Nutzer, der die Nachricht als ungelesen markiert.</param>
        [Transaction]
        public void MarkAsUnread(Notification notification, User user) {
            Require.NotNull(notification, "notification");
            Require.NotNull(user, "user");

            notification.MarkAsUnread();
        }
    }
}