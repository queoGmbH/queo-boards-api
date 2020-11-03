using System;
using Queo.Boards.Core.Infrastructure.NHibernate.Domain;

namespace Queo.Boards.Core.Domain.Activities {
    /// <summary>
    ///     Aktivität zu einem Board
    /// </summary>
    public class BoardActivity : ActivityBase {
        private readonly Board _board;

        /// <summary>
        ///     NHibernate
        /// </summary>
        public BoardActivity() {
        }

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="Entity" />-Klasse.
        /// </summary>
        public BoardActivity(User creator, DateTime creationDate, string text, Board board, ActivityType activityType = ActivityType.Board)
            : base(creator, creationDate, text, activityType) {
            _board = board;
        }

        /// <summary>
        ///     Liefert das <see cref="Board" />
        /// </summary>
        public virtual Board Board {
            get { return _board; }
        }
    }
}