using System.Collections.Generic;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Core.Persistence {
    /// <summary>
    ///     Schnittstelle für Daos für <see cref="Comment" />
    /// </summary>
    public interface ICommentDao : IGenericDao<Comment, int> {
        /// <summary>
        ///     Liefert alle Commentare einer Karte
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        IList<Comment> FindAllOnCard(Card card);


        /// <summary>
        /// Sucht nach Kommentaren auf Karten, die für einen Nutzer zugänglich sind.
        /// </summary>
        /// <param name="pageRequest">Seiteninformationen für das Abrufen</param>
        /// <param name="user">Der Nutzer, für den die Kommentare abgerufen werden sollen.</param>
        /// <param name="searchTerm">Optionale Zeichenfolge, die ein Kommentar im <see cref="Comment.Text"/> enthalten muss, damit er gefunden wird.</param>
        /// <returns></returns>
        IPage<Comment> FindCommentsForUser(IPageable pageRequest, User user, string searchTerm = null);
    }
}