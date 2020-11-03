using System;
using Queo.Boards.Core.Infrastructure.NHibernate.Domain;

namespace Queo.Boards.Core.Domain.Activities {
    /// <summary>
    ///     Basisklasse für Aktivitäten
    /// </summary>
    public class ActivityBase : Entity {
        private readonly ActivityType _activityType;
        private readonly DateTime _creationDate;
        private readonly User _creator;
        private readonly string _text;

        /// <summary>
        ///     NHibernate
        /// </summary>
        public ActivityBase() {
        }

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="Entity" />-Klasse.
        /// </summary>
        public ActivityBase(User creator, DateTime creationDate, string text, ActivityType activityType) {
            _creator = creator;
            _creationDate = creationDate;
            _text = text;
            _activityType = activityType;
        }

        /// <summary>
        ///     Liefert den Aktivitätstypen
        /// </summary>
        public virtual ActivityType ActivityType {
            get { return _activityType; }
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
        ///     Liefert den Text einer Aktivität.
        /// </summary>
        public virtual string Text {
            get { return _text; }
        }
    }
}