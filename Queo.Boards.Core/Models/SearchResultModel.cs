using System.Collections.Generic;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Model mit Boards, Karten und Kommentaren, die bei einer Suche gefunden wurden.
    /// </summary>
    public class SearchResultModel {
        public SearchResultModel(IList<BoardModel> boards, IList<CardModel> cards, IList<CommentModel> comments) {
            Boards = boards;
            Cards = cards;
            Comments = comments;
        }

        /// <summary>
        /// Liefert eine Liste der gefundenen Boards.
        /// </summary>
        public IList<BoardModel> Boards { get; private set; }

        /// <summary>
        /// Liefert eine Liste der gefundenen Karten.
        /// </summary>
        public IList<CardModel> Cards { get; private set; }

        /// <summary>
        /// Liefert eine Liste der gefundenen Kommentare.
        /// </summary>
        public IList<CommentModel> Comments { get; private set; }
    }
}