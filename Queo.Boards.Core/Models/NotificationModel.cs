using System;
using Queo.Boards.Core.Infrastructure.Checks;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Basisklasse für Models, die Benachrichtigungen abbilden.
    /// </summary>
    public abstract class NotificationModel {
        protected NotificationModel(UserModel notificationFor, DateTime createdAt, bool emailSend, DateTime? emailSendAt, bool isMarkedAsRead) {
            Require.NotNull(notificationFor, "notificationFor");

            CreatedAt = createdAt;
            EmailSend = emailSend;
            EmailSendAt = emailSendAt;
            IsMarkedAsRead = isMarkedAsRead;
            NotificationFor = notificationFor;
        }

        /// <summary>
        ///     Ruft den Zeitpunkt der Erstellung der Benachrichtigung ab.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        ///     Ruft ab, ob eine E-Mail für die Benachrichtigung an den Nutzer versendet wurde.
        /// </summary>
        public bool EmailSend { get; set; }

        /// <summary>
        ///     Ruft das Datum ab, wann für die Benachrichtigung eine E-Mail versendet wurde.
        ///     Wurde bisher keine E-Mail versendet (<see cref="EmailSend">EmailSend == false</see>) wird null geliefert.
        /// </summary>
        public DateTime? EmailSendAt { get; set; }

        /// <summary>
        ///     Ruft ab, ob die Benachrichtigung vom Nutzer als gelesen markiert wurde.
        /// </summary>
        public bool IsMarkedAsRead { get; set; }

        /// <summary>
        ///     Ruft ab, für welchen Nutzer die Benachrichtigung erstellt wurde.
        /// </summary>
        public UserModel NotificationFor { get; set; }
    }
}