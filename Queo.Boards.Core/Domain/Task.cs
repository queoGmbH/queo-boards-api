using Queo.Boards.Core.Infrastructure.NHibernate.Domain;

namespace Queo.Boards.Core.Domain {
    /// <summary>
    ///     Eine Aufgabe im Kontext einer <see cref="Checklist" />
    /// </summary>
    public class Task : Entity {
        private readonly Checklist _checklist;
        private bool _isDone;
        private readonly string _title;

        /// <summary>
        ///     NHibernate
        /// </summary>
        public Task() {
        }

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="Entity" />-Klasse.
        /// </summary>
        public Task(Checklist checklist, string title) {
            _checklist = checklist;
            _title = title;
        }

        /// <summary>
        ///     Liefert die Checkliste
        /// </summary>
        public virtual Checklist Checklist {
            get { return _checklist; }
        }

        /// <summary>
        ///     Liefert ob die Aufgabe erledigt ist.
        /// </summary>
        public virtual bool IsDone {
            get { return _isDone; }
        }

        /// <summary>
        ///     Liefert den Titel der Aufgabe
        /// </summary>
        public virtual string Title {
            get { return _title; }
        }

        /// <summary>
        /// Aktualsiert ob der Task abgeschlossen ist.
        /// </summary>
        /// <param name="isDone"></param>
        public virtual void UpdateDone(bool isDone) {
            _isDone = isDone;
        }
    }
}