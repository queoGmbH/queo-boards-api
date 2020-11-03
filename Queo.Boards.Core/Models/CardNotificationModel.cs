using System;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Infrastructure.Checks;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Model, dass eine Benachrichtigung über eine Aktion an einer Karte abbildet.
    /// </summary>
    public class CardNotificationModel : NotificationModel {
        public CardNotificationModel(UserModel notificationFor, CardModel card, CardNotificationReason notificationReason, DateTime createdAt, bool emailSend, DateTime? emailSendAt, bool isMarkedAsRead)
            : base(notificationFor, createdAt, emailSend, emailSendAt, isMarkedAsRead) {
            Require.NotNull(card, "card");

            Card = card;
            NotificationReason = notificationReason;
        }

        /// <summary>
        ///     Ruft die Karte ab, für welche die Benachrichtigung erstellt wurde.
        /// </summary>
        public CardModel Card { get; private set; }

        /// <summary>
        /// Ruft den Anlass für die Benachrichtigung ab.
        /// </summary>
        public CardNotificationReason NotificationReason { get; set; }
    }
}