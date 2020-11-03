using System;
using System.Collections.Generic;
using Queo.Boards.Core.Models.BreadCrumbModels;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Model für eine Liste mit Karten
    /// </summary>
    public class ListWithCardsModel : ListModel {
        private readonly IList<CardModel> _cards;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public ListWithCardsModel(Guid businessId, BoardBreadCrumbModel board, IList<CardModel> cards, string title, int position, DateTime? archivedAt) : base(businessId, board, title, position, archivedAt) {
            _cards = cards;
        }
        
        /// <summary>
        ///     Liefert die Liste der Karten
        /// </summary>
        public IList<CardModel> Cards {
            get { return _cards; }
        }
    }
}