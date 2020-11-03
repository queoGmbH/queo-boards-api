using System.Collections.Generic;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Core.Persistence.Impl {
    /// <summary>
    ///     Dao für <see cref="Checklist" />
    /// </summary>
    public class ChecklistDao : GenericDao<Checklist, int>, IChecklistDao {
        /// <summary>
        ///     Liefert alle Checklisten zu einem Board
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public IList<Checklist> FindAllOnBoard(Board board) {
            return Session.QueryOver<Checklist>()
                    .JoinQueryOver(chk => chk.Card)
                    .Where(x=>!x.IsArchived)
                    .JoinQueryOver(c => c.List)
                    .Where(x=>!x.IsArchived)
                    .Where(x => x.Board == board).List();
        }

        /// <summary>
        ///     Liefert alle Checklisten einer Karte
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public IList<Checklist> FindAllOnCard(Card card) {
            return Session.QueryOver<Checklist>()
                    .Where(x => x.Card == card).List();
        }
    }
}