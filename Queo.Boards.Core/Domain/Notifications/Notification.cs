using System;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Domain;

namespace Queo.Boards.Core.Domain.Notifications {
    /// <summary>
    ///     Stellt die Basisklasse für alle Benachrichtigungen für Nutzer dar.
    ///     Je nachdem wofür eine Benachrichtigung erstellt wird, gibt es Ableitungen für u.a.:
    ///     - BoardNotification (mit einem referenzierten Board)
    ///     - CardNotification (mit einem referenzierter Karte)
    ///     - CommentNotification (mit einem referenzierten Kommentar)
    ///     - evtl. noch andere
    /// </summary>
    /// <remarks>
    ///     Mit Benachrichtigungen sind nicht E-Mails gemeint, sondern allgemeine Benachrichtigungen innerhalb der Anwendung,
    ///     die dem Nutzer innerhalb der Anwendung angezeigt werden.
    ///     Der Nutzer kann konfigurieren, dass er über offene Benachrichtigungen informiert wird.
    /// </remarks>
    public abstract class Notification : Entity {
        private readonly DateTime _creationDateTime;

        private readonly User _notificationFor;

        private bool _emailSend;
        private DateTime? _emailSendAt;
        private bool _isMarkedAsRead;

        /// <summary>
        ///     Konstruktor für NHibernate.
        /// </summary>
        protected Notification() {
        }


        /// <summary>
        /// Ruft den Anzeigenamen für die Benachrichtigung ab.
        /// </summary>
        public abstract string DisplayText { get; }

        /// <summary>
        ///     Standardmäßig zu verwendender Konstruktor.
        /// </summary>
        /// <param name="notificationFor"></param>
        /// <param name="creationDateTime"></param>
        protected Notification(User notificationFor, DateTime creationDateTime)
            : this(notificationFor, creationDateTime, false, null, false) {
        }

        /// <summary>
        ///     Konstruktor zu Vollständigen Instanziierung des Objekts.
        /// </summary>
        /// <param name="notificationFor"></param>
        /// <param name="creationDateTime"></param>
        /// <param name="emailSend"></param>
        /// <param name="isMarkedAsRead"></param>
        protected Notification(User notificationFor, DateTime creationDateTime, bool emailSend, DateTime? emailSendAt, bool isMarkedAsRead) {
            Require.NotNull(notificationFor, "notificationFor");

            _notificationFor = notificationFor;
            _creationDateTime = creationDateTime;
            _emailSend = emailSend;
            _emailSendAt = emailSendAt;
            _isMarkedAsRead = isMarkedAsRead;
        }

        /// <summary>
        ///     Ruft den Zeitpunkt der Erstellung der Benachrichtigung ab.
        /// </summary>
        public virtual DateTime CreationDateTime {
            get { return _creationDateTime; }
        }

        /// <summary>
        ///     Ruft ab, ob eine E-Mail für die Benachrichtigung an den Nutzer versendet wurde.
        /// </summary>
        public virtual bool EmailSend {
            get { return _emailSend; }
        }

        /// <summary>
        ///     Ruft das Datum ab, wann für die Benachrichtigung eine E-Mail versendet wurde.
        ///     Wurde bisher keine E-Mail versendet (<see cref="EmailSend">EmailSend == false</see>) wird null geliefert.
        /// </summary>
        public virtual DateTime? EmailSendAt {
            get { return _emailSendAt; }
        }

        /// <summary>
        ///     Ruft ab, ob die Benachrichtigung vom Nutzer als gelesen markiert wurde.
        /// </summary>
        public virtual bool IsMarkedAsRead {
            get { return _isMarkedAsRead; }
        }

        /// <summary>
        ///     Legt beim Überschreiben fest, welche Entität die Benachrichtigung betrifft.
        ///     Wird als Discriminator für NHibernate verwendet.
        /// </summary>
        public abstract NotificationCategory NotificationCategory { get; }

        /// <summary>
        ///     Ruft ab, für welchen Nutzer die Benachrichtigung erstellt wurde.
        /// </summary>
        public virtual User NotificationFor {
            get { return _notificationFor; }
        }

        /// <summary>
        ///     Setzt die Markierung an der Benachrichtigung, dass eine E-Mail versendet wurde.
        /// </summary>
        /// <returns></returns>
        public virtual void MarkAsEmailSend(DateTime dateTime) {
            _emailSend = true;
            _emailSendAt = dateTime;
        }

        /// <summary>
        ///     Markiert die Benachrichtigung als "nicht gelesen".
        /// </summary>
        public virtual void MarkAsUnread() {
            _isMarkedAsRead = false;
        }

        /// <summary>
        ///     Markiert die Benachrichtigung als "gelesen".
        /// </summary>
        public virtual void MarkAsRead() {
            _isMarkedAsRead = true;
        }
    }
}