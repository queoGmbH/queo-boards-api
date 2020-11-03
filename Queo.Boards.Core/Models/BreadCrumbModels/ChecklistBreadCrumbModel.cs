using System;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;

namespace Queo.Boards.Core.Models.BreadCrumbModels {
    /// <summary>
    ///     Breadcrumb zur Bottom-Up-Navigation von der Checkliste bis zum Board.
    /// </summary>
    public class ChecklistBreadCrumbModel {
        public ChecklistBreadCrumbModel(Guid businessId, string title, CardBreadCrumbModel card) {
            BusinessId = businessId;
            Title = title;
            Card = card;
        }

        public ChecklistBreadCrumbModel(Checklist checklist) {
            Require.NotNull(checklist, "checklist");

            BusinessId = checklist.BusinessId;
            Title = checklist.Title;
            Card = new CardBreadCrumbModel(checklist.Card);
        }

        /// <summary>
        ///     Ruft das BreadCrumbModel für die Karte ab, auf der sich die Checkliste befindet.
        /// </summary>
        public CardBreadCrumbModel Card { get; private set; }

        /// <summary>
        ///     Ruft die Id der Checkliste ab.
        /// </summary>
        public Guid BusinessId { get; private set; }

        /// <summary>
        ///     Ruft den Titel der Checkliste ab.
        /// </summary>
        public string Title { get; private set; }
    }
}