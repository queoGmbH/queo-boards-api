using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Domain;

namespace Queo.Boards.Core.Domain {
    /// <summary>
    ///     Eine Karte
    /// </summary>
    public class Card : Entity {
        private readonly IList<User> _assignedUsers = new List<User>();
        private readonly IList<Checklist> _checklists = new List<Checklist>();
        private readonly IList<Comment> _comments = new List<Comment>();
        private readonly IList<CardNotification> _cardNotifications = new List<CardNotification>();
        private readonly DateTime _createdAt;

        private readonly User _createdBy;
        private readonly IList<Document> _documents = new List<Document>();
        private DateTime? _archivedAt;
        private string _description;
        private DateTime? _due;
        private bool _dueExpirationNotificationCreated;
        private DateTime? _dueExpirationNotificationCreatedAt;
        private bool _isArchived;
        private IList<Label> _labels = new List<Label>();
        private List _list;

        private string _title;

        /// <summary>
        ///     NHibernate
        /// </summary>
        public Card() {
        }

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="Entity" />-Klasse.
        /// </summary>
        public Card(List list, string title, string description, DateTime? due, IList<Label> labels, EntityCreatedDto entityCreatedDto) {
            _list = list;
            _title = title;
            _description = description;
            _due = due;
            if (labels != null && labels.Any()) {
                _labels = labels.ToList();
            }

            _createdAt = entityCreatedDto.CreatedAt;
            _createdBy = entityCreatedDto.CreatedBy;
        }

        /// <summary>
        ///     Ruft ab, wann die Karte archiviert wurde.
        ///     Wurde die Karte bisher nicht archiviert, wird NULL geliefert.
        /// </summary>
        public virtual DateTime? ArchivedAt {
            get { return _archivedAt; }
        }

        /// <summary>
        ///     Ruft eine schreibgeschützte Kopie der Liste mit Nutzern ab, die dieser Karte zugewiesen sind.
        /// </summary>
        public virtual IList<User> AssignedUsers {
            get { return new ReadOnlyCollection<User>(_assignedUsers); }
        }

        /// <summary>
        ///     Liefert die Checklisten
        /// </summary>
        public virtual IList<Checklist> Checklists {
            get { return _checklists; }
        }

        /// <summary>
        ///     Liefert die Kommentare einer Karte
        /// </summary>
        public virtual IList<Comment> Comments {
            get { return _comments; }
        }

        /// <summary>
        ///     Ruft ab, wann die Karte erstellt wurde.
        /// </summary>
        public virtual DateTime CreatedAt {
            get { return _createdAt; }
        }

        /// <summary>
        ///     Ruft an, von welchem Nutzer die Karte erstellt wurde.
        /// </summary>
        public virtual User CreatedBy {
            get { return _createdBy; }
        }

        /// <summary>
        ///     Liefert die Beschreibung der Karte
        /// </summary>
        public virtual string Description {
            get { return _description; }
        }

        /// <summary>
        ///     Liefert die Dokumente der Karte
        /// </summary>
        public virtual IList<Document> Documents {
            get { return _documents; }
        }

        /// <summary>
        ///     Liefert das Fälligkeitsdatum
        /// </summary>
        public virtual DateTime? Due {
            get { return _due; }
        }

        /// <summary>
        ///     Ruft ab, ob <see cref="CardNotification">Benachrichtigungen</see> für das Überschreiten des
        ///     <see cref="Due">Fälligkeitsdatums der Karte</see> erstellt wurden.
        ///     Beim Verschieben der Fälligkeit, wird der Wert auf false zurückgesetzt.
        /// </summary>
        public virtual bool DueExpirationNotificationCreated {
            get { return _dueExpirationNotificationCreated; }
        }

        /// <summary>
        ///     Ruft ab, wann die <see cref="CardNotification">Benachrichtigungen</see> für das Überschreiten des
        ///     <see cref="Due">Fälligkeitsdatums der Karte</see> erstellt wurden.
        ///     Wurden bisher keine Benachrichtigungen erstellt, wird null geliefert.
        ///     Beim Verschieben der Fälligkeit, wird das Datum auf NULL zurückgesetzt.
        /// </summary>
        public virtual DateTime? DueExpirationNotificationCreatedAt {
            get { return _dueExpirationNotificationCreatedAt; }
        }

        /// <summary>
        ///     Liefert ob die Karte archiviert ist.
        /// </summary>
        public virtual bool IsArchived {
            get { return _isArchived; }
        }

        /// <summary>
        ///     Liefert die Labels
        /// </summary>
        public virtual IList<Label> Labels {
            get { return _labels; }
        }

        /// <summary>
        ///     Liefert die Benachrichtigungen, die zu dieser Karte erstellt wurden.
        /// </summary>
        public virtual IList<CardNotification> CardNotifications {
            get {
                return _cardNotifications;
            }
        }

        /// <summary>
        ///     Liefert die Liste
        /// </summary>
        public virtual List List {
            get { return _list; }
        }

        /// <summary>
        ///     Liefert den Titel
        /// </summary>
        public virtual string Title {
            get { return _title; }
        }

        /// <summary>
        ///     Fügt der Karte ein Label hinzu
        /// </summary>
        /// <param name="label"></param>
        public virtual void AddLabel(Label label) {
            if (_labels == null) {
                _labels = new List<Label>();
            }
            _labels.Add(label);
        }

        /// <summary>
        ///     Archiviert die Karte.
        /// </summary>
        /// <param name="archivedAt"></param>
        public virtual void Archive(DateTime archivedAt) {
            _isArchived = true;
            _archivedAt = archivedAt;
        }

        /// <summary>
        ///     Weißt diese Karte einem Nutzer zu.
        /// </summary>
        /// <param name="user"></param>
        public virtual void AssignUser(User user) {
            Require.NotNull(user, "user");
            if (!_assignedUsers.Contains(user)) {
                _assignedUsers.Add(user);
            }
        }

        /// <summary>
        /// Entfernt alle der Karte zugewiesenen Nutzer.
        /// </summary>
        public virtual void ClearAssignedUsers() {
            _assignedUsers.Clear();
        }

        /// <summary>
        ///     Liefert die Position der Karte in der Liste.
        /// </summary>
        public virtual int GetPositionInList() {
            return _list.Cards.IndexOf(this);
        }

        /// <summary>
        ///     Hebt die Markierung der Karte als, "Benachrichtigungen über Ablauf der Fälligkeit der Karte" erstellt, auf und
        ///     setzt die Werte <see cref="DueExpirationNotificationCreated" /> und
        ///     <see cref="DueExpirationNotificationCreatedAt" /> zurück.
        /// </summary>
        public virtual void ResetDueExpirationNotificationCreated() {
            _dueExpirationNotificationCreated = false;
            _dueExpirationNotificationCreatedAt = null;
        }

        /// <summary>
        ///     Stellt die Karte wieder her und hebt damit die Archivierung auf.
        /// </summary>
        public virtual void Restore() {
            _isArchived = false;
            _archivedAt = null;
        }

        /// <summary>
        ///     Entfernt den Nutzer aus <see cref="AssignedUsers">der Liste der zugeordneten Nutzer</see>.
        /// </summary>
        /// <param name="user"></param>
        public virtual void UnassignUser(User user) {
            Require.NotNull(user, "user");

            if (_assignedUsers.Contains(user)) {
                _assignedUsers.Remove(user);
            }
        }

        /// <summary>
        ///     Aktualisiert die Beschreibung einer Karte
        /// </summary>
        /// <param name="description"></param>
        public virtual void UpdateDescription(string description) {
            _description = description;
        }

        /// <summary>
        ///     Aktualisiert das Fälligkeitsdatum.
        ///     NULL, wenn es kein Fälligkeitsdatum gibt.
        /// </summary>
        /// <param name="due"></param>
        public virtual void UpdateDue(DateTime? due) {
            _due = due;
        }

        /// <summary>
        ///     Markiert die Karte als, "Benachrichtigungen über Ablauf der Fälligkeit der Karte" erstellt.
        /// </summary>
        /// <param name="dueExpirationNotificationCreatedAt"></param>
        public virtual void UpdateDueExpirationNotificationCreated(DateTime dueExpirationNotificationCreatedAt) {
            _dueExpirationNotificationCreated = true;
            _dueExpirationNotificationCreatedAt = dueExpirationNotificationCreatedAt;
        }

        /// <summary>
        ///     Aktualisiert das Elternelement
        /// </summary>
        /// <param name="list"></param>
        public virtual void UpdateParent(List list) {
            _list = list;
        }

        /// <summary>
        ///     Aktualisiert die Karte
        /// </summary>
        /// <param name="title"></param>
        public virtual void UpdateTitle(string title) {
            _title = title;
        }
    }
}