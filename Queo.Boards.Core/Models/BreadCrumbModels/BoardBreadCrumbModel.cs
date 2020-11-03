using System;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;

namespace Queo.Boards.Core.Models.BreadCrumbModels {
    /// <summary>
    ///     Breadcrumb für das Board.
    /// </summary>
    public class BoardBreadCrumbModel {
        public BoardBreadCrumbModel(Guid businessId, string title) {
            BusinessId = businessId;
            Title = title;
        }

        public BoardBreadCrumbModel(Board board) {
            Require.NotNull(board, "board");

            BusinessId = board.BusinessId;
            Title = board.Title;
        }

        /// <summary>
        ///     Ruft die Id des Boards ab.
        /// </summary>
        public Guid BusinessId { get; private set; }

        /// <summary>
        ///     Ruft den Titel des Boards ab.
        /// </summary>
        public string Title { get; private set; }
    }
}