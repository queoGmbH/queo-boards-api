using System;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;

namespace Queo.Boards.Core.Models.BreadCrumbModels {
    /// <summary>
    ///     Breadcrumb zur Bottom-Up-Navigation von der Karte bis zum Board.
    /// </summary>
    public class CardBreadCrumbModel {
        public CardBreadCrumbModel(Guid businessId, string title, ListBreadCrumbModel list) {
            BusinessId = businessId;
            Title = title;
            List = list;
        }

        public CardBreadCrumbModel(Card card) {
            Require.NotNull(card, "card");

            BusinessId = card.BusinessId;
            Title = card.Title;
            List = new ListBreadCrumbModel(card.List);
        }

        /// <summary>
        ///     Ruft die Id der Karte ab.
        /// </summary>
        public Guid BusinessId { get; private set; }

        /// <summary>
        ///     Ruft das BreadCrumbModel für die Liste ab, auf der sich die Karte befindet.
        /// </summary>
        public ListBreadCrumbModel List { get; private set; }

        /// <summary>
        ///     Ruft den Titel der Karte ab.
        /// </summary>
        public string Title { get; private set; }
    }
}