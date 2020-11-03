using System;
using Queo.Boards.Core.Infrastructure.Checks;

namespace Queo.Boards.Core.Domain.Notifications {
    /// <summary>
    ///     Bildet eine Benachrichtigung für einen Nutzer ab, das etwas ein einem Kommentar vorgefallen ist.
    /// </summary>
    public class CommentNotification : Notification {
        private readonly Comment _comment;

        private readonly CommentNotificationReason _notificationReason;

        /// <summary>
        ///     Konstruktor für NHibernate.
        /// </summary>
        public CommentNotification() {
        }

        /// <summary>
        ///     Konstruktor der standardmäßig verwendet werden sollte, um initial die Benachrichtigung zu erstellen, die noch nicht
        ///     als gelesen markiert wurde und zu der keine E-Mail versendet wurde.
        /// </summary>
        /// <param name="comment">Der Kommentar, für welchen die Benachrichtigung erstellt wurde.</param>
        /// <param name="notificationReason">Der Grund für die Benachrichtigung.</param>
        /// <param name="notificationFor">Für welchen Nutzer wurde die Benachrichtigung erstellt.</param>
        /// <param name="creationDateTime">Wann wurde die Benachrichtigung erstellt.</param>
        public CommentNotification(Comment comment, CommentNotificationReason notificationReason, User notificationFor, DateTime creationDateTime)
            : this(comment, notificationReason, notificationFor, creationDateTime, false, null, false) {
        }

        /// <summary>
        ///     Konstruktor zur vollständigen Initialisierung der Benachrichtigung für eine Karte.
        /// </summary>
        /// <param name="comment">Der Kommentar, für welchen die Benachrichtigung erstellt wurde.</param>
        /// <param name="notificationReason">Der Grund für die Benachrichtigung.</param>
        /// <param name="notificationFor">Für welchen Nutzer wurde die Benachrichtigung erstellt.</param>
        /// <param name="creationDateTime">Wann wurde die Benachrichtigung erstellt.</param>
        /// <param name="emailSend">Wurde eine E-Mail für die Benachrichtigung versendet.</param>
        /// <param name="emailSendAt">Wann wurde die E-Mail versendet?</param>
        /// <param name="isMarkedAsRead">Wurde die Nachricht als gelesen markiert.</param>
        public CommentNotification(Comment comment, CommentNotificationReason notificationReason, User notificationFor, DateTime creationDateTime, bool emailSend, DateTime? emailSendAt, bool isMarkedAsRead)
            : base(notificationFor, creationDateTime, emailSend, emailSendAt, isMarkedAsRead) {
            Require.NotNull(comment, "comment");

            _comment = comment;
            _notificationReason = notificationReason;
        }

        /// <summary>
        ///     Ruft den Kommentar ab, zu welchem die Benachrichtigung erstellt wurde.
        /// </summary>
        public virtual Comment Comment {
            get { return _comment; }
        }

        /// <summary>
        ///     Ruft den Anzeigenamen für die Benachrichtigung ab.
        /// </summary>
        public override string DisplayText {
            get { return string.Format("{0} for {1} => {2}", NotificationCategory, NotificationFor.UserName, NotificationReason); }
        }

        /// <summary>
        ///     Legt beim Überschreiben fest, welche Entität die Benachrichtigung betrifft.
        ///     Wird als Discriminator für NHibernate verwendet.
        /// </summary>
        public override NotificationCategory NotificationCategory {
            get { return NotificationCategory.Comment; }
        }

        /// <summary>
        ///     Ruft den Anlass für die Benachrichtigung ab.
        /// </summary>
        public virtual CommentNotificationReason NotificationReason {
            get { return _notificationReason; }
        }
    }

    /// <summary>
    ///     Auflistung der Arten bzw. Gründe für die Benachrichtigung über einen Kommentar.
    /// </summary>
    public enum CommentNotificationReason {
        /// <summary>
        ///     Ein Kommentar wurde erstellt.
        /// </summary>
        CommentCreated
    }
}