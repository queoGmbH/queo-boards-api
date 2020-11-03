using System;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;

namespace Queo.Boards.Core.Models.BreadCrumbModels {
    /// <summary>
    ///     Breadcrumb zur Bottom-Up-Navigation von der Liste bis zum Board.
    /// </summary>
    public class ListBreadCrumbModel {
        public ListBreadCrumbModel(Guid businessId, string title, BoardBreadCrumbModel board) {
            BusinessId = businessId;
            Title = title;
            Board = board;
        }

        public ListBreadCrumbModel(List list) {
            Require.NotNull(list, "list");

            BusinessId = list.BusinessId;
            Title = list.Title;
            Board = new BoardBreadCrumbModel(list.Board);
        }

        /// <summary>
        ///     Ruft das BreadCrumbModel für das Board ab, auf der sich die Liste befindet.
        /// </summary>
        public BoardBreadCrumbModel Board { get; private set; }

        /// <summary>
        ///     Ruft die Id der Liste ab.
        /// </summary>
        public Guid BusinessId { get; private set; }

        /// <summary>
        ///     Ruft den Titel der Liste ab.
        /// </summary>
        public string Title { get; private set; }
    }
}