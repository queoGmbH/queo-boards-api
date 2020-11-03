using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Models.Builders {
    /// <summary>
    ///     Builder für <see cref="BoardSummaryModel" />
    /// </summary>
    public static class BoardSummaryModelBuilder {
        /// <summary>
        ///     Erstellt ein neues <see cref="BoardSummaryModel" />
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static BoardSummaryModel Build(Board board) {
            return new BoardSummaryModel(board.BusinessId, board.Title, board.Accessibility, board.ColorScheme, board.IsPrivate, board.CreatedAt, board.IsTemplate, board.IsArchived, board.ArchivedAt);
        }
    }
}