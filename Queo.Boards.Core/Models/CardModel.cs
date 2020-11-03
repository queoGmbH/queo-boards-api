using System;
using System.Collections.Generic;
using Queo.Boards.Core.Models.BreadCrumbModels;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Model für die grundlegenden Kartendaten
    /// </summary>
    public class CardModel : EntityModel {
        private readonly IList<LabelModel> _assignedLabels;
        private readonly IList<UserModel> _assignedUsers;
        private readonly int _attachmentsCount;
        private readonly DateTime? _archivedAt;
        private readonly int _commentCount;
        private readonly string _description;
        private readonly DateTime? _due;
        private readonly DateTime _createdAt;
        private readonly ListBreadCrumbModel _list;
        private readonly int _position;
        private readonly int _tasksDoneCount;
        private readonly int _tasksOverallCount;
        private readonly string _title;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public CardModel(Guid businessId, string title, string description, ListBreadCrumbModel list, int position, DateTime createdAt, DateTime? due, IList<LabelModel> assignedLabels, int commentCount, int tasksOverallCount, int tasksDoneCount, IList<UserModel> assignedUsers, int attachmentsCount, DateTime? archivedAt)
            : base(businessId) {
            _title = title;
            _description = description;
            _list = list;
            _position = position;
            _due = due;
            _assignedLabels = assignedLabels;
            _commentCount = commentCount;
            _tasksOverallCount = tasksOverallCount;
            _tasksDoneCount = tasksDoneCount;
            _assignedUsers = assignedUsers;
            _attachmentsCount = attachmentsCount;
            _archivedAt = archivedAt;
            _createdAt = createdAt;
        }

        /// <summary>
        /// Ruft das Archivierungsdatum der Karte ab oder null wenn die Karte nicht archiviert ist.
        /// </summary>
        public DateTime? ArchivedAt {
            get { return _archivedAt; }
        }

        /// <summary>
        ///     Liefert die zugeordneten Labels
        /// </summary>
        public IList<LabelModel> AssignedLabels {
            get { return _assignedLabels; }
        }

        /// <summary>
        /// Ruft ab, wann die Karte erstellt wurde.
        /// </summary>
        public DateTime CreatedAt {
            get { return _createdAt; }
        }

        /// <summary>
        ///     Liefert die zugewiesenen Nutzer
        /// </summary>
        public IList<UserModel> AssignedUsers {
            get { return _assignedUsers; }
        }

        /// <summary>
        ///     Liefert die Anzahl an Anhängen
        /// </summary>
        public int AttachmentsCount {
            get { return _attachmentsCount; }
        }

        /// <summary>
        ///     Liefert die Anzahl Kommentare
        /// </summary>
        public int CommentCount {
            get { return _commentCount; }
        }

        /// <summary>
        ///     Liefert die Beschreibung
        /// </summary>
        public string Description {
            get { return _description; }
        }

        /// <summary>
        ///     Liefert das Fälligkeitsdatum oder NULL
        /// </summary>
        public DateTime? Due {
            get { return _due; }
        }

        /// <summary>
        ///     BreadCrumb der Liste, der die Karte zugeordnet ist.
        /// </summary>
        public ListBreadCrumbModel List {
            get { return _list; }
        }

        /// <summary>
        ///     Liefert die Position der Karte innerhalb der Liste
        /// </summary>
        public int Position {
            get { return _position; }
        }

        /// <summary>
        ///     Liefert die Anzahl abgeschlossener ToDos über alle Checklisten
        /// </summary>
        public int TasksDoneCount {
            get { return _tasksDoneCount; }
        }

        /// <summary>
        ///     Liefert die Gesamtanzahl ToDos über alle Checklisten
        /// </summary>
        public int TasksOverallCount {
            get { return _tasksOverallCount; }
        }

        /// <summary>
        ///     Liefert den Titel
        /// </summary>
        public string Title {
            get { return _title; }
        }
    }
}