using System;
using System.Collections.Generic;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Models.BreadCrumbModels;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Model einer <see cref="Checklist" />
    /// </summary>
    public class ChecklistModel : EntityModel {
        private readonly CardBreadCrumbModel _card;
        private readonly IList<TaskModel> _tasks;
        private readonly string _title;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public ChecklistModel(Guid businessId, CardBreadCrumbModel card, string title, IList<TaskModel> tasks)
            : base(businessId) {
            _card = card;
            _title = title;
            _tasks = tasks;
        }

        /// <summary>
        ///     Liefert den BreadCrumb der Karte
        /// </summary>
        public CardBreadCrumbModel Card {
            get { return _card; }
        }

        /// <summary>
        ///     Liefert die Aufgaben der Checkliste
        /// </summary>
        public IList<TaskModel> Tasks {
            get { return _tasks; }
        }

        /// <summary>
        ///     Liefert den Titel
        /// </summary>
        public string Title {
            get { return _title; }
        }
    }
}