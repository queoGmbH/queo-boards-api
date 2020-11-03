using System.Collections.Generic;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Core.Services {
    /// <summary>
    ///     Schnittstelle für Services für <see cref="Comment" />
    /// </summary>
    public interface ICommentService {
        /// <summary>
        ///     Erstellt eine neue Karte
        /// </summary>
        /// <param name="card"></param>
        /// <param name="text"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        Comment Create(Card card, string text, User creator);

        /// <summary>
        ///     Liefert alle Kommentare einer Karte
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        IList<Comment> FindAllOnCard(Card card);

        /// <summary>
        ///     Aktualisiert einen Kommentar
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Comment UpdateText(Comment comment, string value);

        /// <summary>
        /// Aktualisiert den Löschzustand eines Kommentars
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        Comment UpdateIsDeleted(Comment comment, bool isDeleted);
        /// Erstellt eine Kopie eines Kommentars.
        /// </summary>
        /// <param name="sourceComment">Der zu kopierende Kommentar.</param>
        /// <param name="targetCard">Die Karte, an welche der Kommentar kopiert werden soll.</param>
        /// <returns></returns>
        Comment Copy(Comment sourceComment, Card targetCard);

        /// <summary>
        /// Sucht seitenweise nach Kommentaren, auf Karten, welche für den Nutzer zugänglich sind.
        /// </summary>
        /// <param name="pageRequest">Seiteninformationen</param>
        /// <param name="user">Der Nutzer für den die Kommentare abgerufen werden sollen.</param>
        /// <param name="query">Optionale Suchzeichenfolge zur Einschränkung der gefundenen Kommentare.</param>
        /// <returns></returns>
        IPage<Comment> FindCommentsForUser(IPageable pageRequest, User user, string query);
    }
}