using System.Collections.Generic;
using Queo.Boards.Core.Infrastructure.NHibernate.Domain;

namespace Queo.Boards.Core.Domain {
    /// <summary>
    ///     Eine Checkliste im Sinne des Parents für die Checklist Items
    /// </summary>
    public class Checklist : Entity {
        private readonly Card _card;

        private IList<Task> _tasks;
        private string _title;

        /// <summary>
        ///     NHibernate
        /// </summary>
        public Checklist() {
        }

        /// <summary>
        /// </summary>
        /// <param name="card"></param>
        /// <param name="title"></param>
        public Checklist(Card card, string title) {
            _card = card;
            _title = title;
            _tasks = new List<Task>();
        }

        /// <summary>
        ///     Liefert die Karte
        /// </summary>
        public virtual Card Card {
            get { return _card; }
        }
        /// <summary>
        ///     Liefert die <see cref="Task" />s der Checkliste
        /// </summary>
        public virtual IList<Task> Tasks {
            get { return _tasks; }
        }

        /// <summary>
        ///     Liefert den Titel
        /// </summary>
        public virtual string Title {
            get { return _title; }
        }

        /// <summary>
        ///     Aktualisiert den Titel
        /// </summary>
        /// <param name="title"></param>
        public virtual void Update(string title) {
            _title = title;
        }
    }
}