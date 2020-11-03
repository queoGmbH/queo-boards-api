using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Models.BreadCrumbModels;

namespace Queo.Boards.Core.Models.Builders {

    /// <summary>
    /// Builder zum Erstellen der Models für BreadCrumbs.
    /// </summary>
    public static class BreadCrumbsModelBuilder {


        public static BoardBreadCrumbModel Build(Board board) {
            return new BoardBreadCrumbModel(board.BusinessId, board.Title);
        }

        public static ListBreadCrumbModel Build(List list) {
            return new ListBreadCrumbModel(list.BusinessId, list.Title, Build(list.Board));
        }

        public static CardBreadCrumbModel Build(Card card) {
            return new CardBreadCrumbModel(card.BusinessId, card.Title, Build(card.List));
        }

        public static ChecklistBreadCrumbModel Build(Checklist checklist) {
            return new ChecklistBreadCrumbModel(checklist.BusinessId, checklist.Title, Build(checklist.Card));
        }

    }
}