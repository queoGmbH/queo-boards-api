using System;
using System.Collections.Generic;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Infrastructure.NHibernate.Domain;

namespace Queo.Boards.Core.Domain {
    /// <summary>
    ///     Ein Kommentar einer Karte
    /// </summary>
    public class Comment : Entity {
        private readonly Card _card;
        private readonly DateTime _creationDate;
        private readonly User _creator;
        private bool _isDeleted;
        private string _text;

        private readonly IList<CommentNotification> _commentNotifications = new List<CommentNotification>();

        /// <summary>
        ///     NHibernate
        /// </summary>
        public Comment() {
        }

        /// <summary>
        /// Ruft die Benachrichtigungen ab, die für diesen Kommentar erstellt wurden.
        /// </summary>
        public virtual IList<CommentNotification> CommentNotifications {
            get { return _commentNotifications; }
        }

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="Entity" />-Klasse.
        /// </summary>
        public Comment(Card card, string text, User creator, DateTime creationDate) {
            _card = card;
            _text = text;
            _creationDate = creationDate;
            _creator = creator;
        }

        /// <summary>
        ///     Liefert die Karte
        /// </summary>
        public virtual Card Card {
            get { return _card; }
        }

        /// <summary>
        ///     Liefert das Erstellungsdatum
        /// </summary>
        public virtual DateTime CreationDate {
            get { return _creationDate; }
        }
        /// <summary>
        ///     Liefert den Ersteller
        /// </summary>
        public virtual User Creator {
            get { return _creator; }
        }

        /// <summary>
        ///     Liefert ob ein Kommentar als gelöscht markiert ist.
        /// </summary>
        public virtual bool IsDeleted {
            get { return _isDeleted; }
        }

        /// <summary>
        ///     Liefert den Text
        /// </summary>
        public virtual string Text {
            get { return _text; }
        }

        /// <summary>
        ///     Aktualisiert den Löschzustand einer Karte
        /// </summary>
        /// <param name="isDeleted"></param>
        public virtual void UpdateIsDeleted(bool isDeleted) {
            _isDeleted = isDeleted;
        }

        /// <summary>
        ///     Aktualisiert den Text des Kommentars
        /// </summary>
        /// <param name="newText"></param>
        public virtual void UpdateText(string newText) {
            _text = newText;
        }
    }
}