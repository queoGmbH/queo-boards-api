using System;
using Queo.Boards.Core.Infrastructure.Checks;

namespace Queo.Boards.Core.Domain.Notifications {
    /// <summary>
    ///     Bildet eine Benachrichtigung zu einer Karte ab.
    /// </summary>
    public class CardNotification : Notification {
        private readonly Card _card;
        private readonly CardNotificationReason _notificationReason;


        /// <summary>
        ///     Konstruktor für NHibernate.
        /// </summary>
        public CardNotification() {
        }

        /// <summary>
        ///     Konstruktor der Standardmäßig verwendet werden sollte, um initial die Benachrichtigung zu erstellen, die noch nicht
        ///     als gelesen markiert wurde und zu der keine E-Mail versendet wurde.
        /// </summary>
        /// <param name="card">Die Karte für welche die Benachrichtigung erstellt wurde.</param>
        /// <param name="notificationFor">Für welchen Nutzer wurde die Benachrichtigung erstellt.</param>
        /// <param name="creationDateTime">Wann wurde die Benachrichtigung erstellt.</param>
        public CardNotification(Card card, CardNotificationReason notificationReason, User notificationFor, DateTime creationDateTime)
            : this(card, notificationReason, notificationFor, creationDateTime, false, null, false) {
        }

        /// <summary>
        ///     Konstruktor zur vollständigen Initialisierung der Benachrichtigung für eine Karte.
        /// </summary>
        /// <param name="card">Die Karte, für welche die Benachrichtigung erstellt wurde.</param>
        /// <param name="notificationFor">Für welchen Nutzer wurde die Benachrichtigung erstellt.</param>
        /// <param name="creationDateTime">Wann wurde die Benachrichtigung erstellt.</param>
        /// <param name="emailSend">Wurde eine E-Mail für die Benachrichtigung versendet.</param>
        /// <param name="emailSendAt">Wann wurde die E-Mail versendet.</param>
        /// <param name="isMarkedAsRead">Wurde die Nachricht als gelesen markiert.</param>
        public CardNotification(Card card, CardNotificationReason notificationReason, User notificationFor, DateTime creationDateTime, bool emailSend, DateTime? emailSendAt, bool isMarkedAsRead)
            : base(notificationFor, creationDateTime, emailSend, emailSendAt, isMarkedAsRead) {
            Require.NotNull(card, "card");

            _card = card;
            _notificationReason = notificationReason;
        }

        /// <summary>
        /// Ruft den Anlass für die Benachrichtigung ab.
        /// </summary>
        public virtual CardNotificationReason NotificationReason {
            get { return _notificationReason; }
        }

        /// <summary>
        ///     Ruft die Karte ab, zu welcher die Benachrichtigung erfolgte.
        /// </summary>
        public virtual Card Card {
            get { return _card; }
        }

        /// <summary>
        /// Ruft den Anzeigenamen für die Benachrichtigung ab.
        /// </summary>
        public override string DisplayText {
            get {
                return string.Format("{0} for {1} => {2}", NotificationCategory, NotificationFor.UserName, NotificationReason);
            }
        }

        /// <summary>
        ///     Legt beim Überschreiben fest, welche Entität die Benachrichtigung betrifft.
        ///     Wird als Discriminator für NHibernate verwendet.
        /// </summary>
        public override NotificationCategory NotificationCategory {
            get { return NotificationCategory.Card; }
        }
    }
}