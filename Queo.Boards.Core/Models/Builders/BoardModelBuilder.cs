using System.Collections.Generic;
using System.Linq;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Models.Builders {
    /// <summary>
    ///     Model Builder für <see cref="BoardModel" />
    /// </summary>
    public class BoardModelBuilder {
        /// <summary>
        ///     Erstellt ein neues <see cref="BoardModel" />
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static BoardModel Build(Board board) {
            IList<CardModel> allCards = new List<CardModel>();
            IList<ListModel> lists = new List<ListModel>();

            if (board.Lists != null && board.Lists.Any()) {
                IEnumerable<List> notArchivedLists = board.Lists.Where(x => x != null && !x.IsArchived);
                IEnumerable<Card> cards = notArchivedLists.SelectMany(x => x.Cards).Where(x => x != null);
                IEnumerable<Card> notArchivedCards = cards.Where(x => !x.IsArchived);
                allCards = notArchivedCards.Select(CardModelBuilder.Build).ToList();
            }

            if (board.Lists != null && board.Lists.Any()) {
                lists = board.Lists.Where(x => x != null && !x.IsArchived).Select(ListModelBuilder.Build).ToList();
            }

            IList<LabelModel> labels = board.Labels.Select(LabelModelBuilder.Build).ToList();
            IList<UserModel> members = board.Members.Select(UserModelBuilder.BuildUser).ToList();
            IList<UserModel> owners = board.Owners.Select(UserModelBuilder.BuildUser).ToList();
            IList<TeamSummaryModel> teams = board.Teams.Select(TeamModelBuilder.BuildSummary).ToList();
            IList<UserModel> boardUsers = board.GetBoardUsers().Select(UserModelBuilder.BuildUser).ToList();

            return new BoardModel(BoardSummaryModelBuilder.Build(board), lists, allCards, labels, owners, members, teams, boardUsers);
        }

        public static BoardModel Build(Board board, IList<List> lists) {
            IList<CardModel> cardModels = lists.Where(list => !list.IsArchived).SelectMany(list => list.Cards).Distinct().Where(card => !card.IsArchived)
                .Select(CardModelBuilder.Build).ToList();
            IList<ListModel> listModels = lists.Where(list => !list.IsArchived).Distinct().Select(ListModelBuilder.Build).ToList();

            IList<LabelModel> labels = board.Labels.Select(LabelModelBuilder.Build).ToList();
            IList<UserModel> members = board.Members.Select(UserModelBuilder.BuildUser).ToList();
            IList<UserModel> owners = board.Owners.Select(UserModelBuilder.BuildUser).ToList();
            IList<TeamSummaryModel> teams = board.Teams.Select(TeamModelBuilder.BuildSummary).ToList();
            IList<UserModel> boardUsers = board.GetBoardUsers().Select(UserModelBuilder.BuildUser).ToList();

            return new BoardModel(BoardSummaryModelBuilder.Build(board), listModels, cardModels, labels, owners, members, teams, boardUsers);
        }
    }
}