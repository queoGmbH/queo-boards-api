using System.Collections.Generic;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     DTO für Board Summary und Listen
    /// </summary>
    public class BoardModel : EntityModel {
        private readonly IList<UserModel> _boardUsers;
        private readonly IList<CardModel> _cards;
        private readonly IList<LabelModel> _labels;
        private readonly IList<ListModel> _lists;
        private readonly IList<UserModel> _members;
        private readonly IList<UserModel> _owners;
        private readonly BoardSummaryModel _summary;
        private readonly IList<TeamSummaryModel> _teams;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public BoardModel(BoardSummaryModel summary, IList<ListModel> lists, IList<CardModel> cards, IList<LabelModel> labels, IList<UserModel> owners, IList<UserModel> members, IList<TeamSummaryModel> teams, IList<UserModel> boardUsers)
            : base(summary.BusinessId) {
            _summary = summary;
            _lists = lists;
            _cards = cards;
            _labels = labels;
            _members = members;
            _teams = teams;
            _boardUsers = boardUsers;
            _owners = owners;
        }

        /// <summary>
        ///     Ruft alle Nutzer des Boards ab.
        ///     Damit sind alle Nutzer gemeint, die lesenden Zugriff auf das Board haben.
        /// </summary>
        public IList<UserModel> BoardUsers {
            get { return _boardUsers; }
        }

        /// <summary>
        ///     Liefert die Karten aller Listen
        /// </summary>
        public IList<CardModel> Cards {
            get { return _cards; }
        }

        /// <summary>
        ///     Liefert die Labels
        /// </summary>
        public IList<LabelModel> Labels {
            get { return _labels; }
        }

        /// <summary>
        ///     Liefert die Listen des Boards
        /// </summary>
        public IList<ListModel> Lists {
            get { return _lists; }
        }

        /// <summary>
        ///     Ruft die zugewiesenen Nutzer ab.
        /// </summary>
        public IList<UserModel> Members {
            get { return _members; }
        }

        /// <summary>
        ///     Ruft die Besitzer des Boards ab. Es gibt immer mindestens einen Besitzer.
        /// </summary>
        public IList<UserModel> Owners {
            get { return _owners; }
        }

        /// <summary>
        ///     Liefert die Board-Zusammenfassung in Form eines <see cref="BoardSummaryModel" />
        /// </summary>
        public BoardSummaryModel Summary {
            get { return _summary; }
        }

        /// <summary>
        ///     Ruft die Teams ab, die dem Board zugeordnet sind.
        /// </summary>
        public IList<TeamSummaryModel> Teams {
            get { return _teams; }
        }
    }
}