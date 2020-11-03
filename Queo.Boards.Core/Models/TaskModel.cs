using System;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Models.BreadCrumbModels;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Model eines <see cref="Task" />
    /// </summary>
    public class TaskModel : EntityModel {
        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public TaskModel(Guid businessId, ChecklistBreadCrumbModel checklist, string title, bool isDone)
            : base(businessId) {
            Checklist = checklist;
            Title = title;
            IsDone = isDone;
        }

        /// <summary>
        ///     Liefert den Breadcrumb für die Checkliste
        /// </summary>
        public ChecklistBreadCrumbModel Checklist { get; private set; }

        /// <summary>
        ///     Liefert ob die Aufgabe abgeschlossen ist.
        /// </summary>
        public bool IsDone { get; private set; }

        /// <summary>
        ///     LIefert den Titel
        /// </summary>
        public string Title { get; private set; }
    }
}