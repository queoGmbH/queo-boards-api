using System.Collections.Generic;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Core.Persistence {
    /// <summary>
    ///     Schnittstelle für Daos für <see cref="Checklist" />
    /// </summary>
    public interface IChecklistDao : IGenericDao<Checklist, int> {
        /// <summary>
        ///     Liefert alle Checklisten zu einem Board
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        IList<Checklist> FindAllOnBoard(Board board);

        /// <summary>
        ///     Liefert alle Checklisten einer Karte
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        IList<Checklist> FindAllOnCard(Card card);
    }
}