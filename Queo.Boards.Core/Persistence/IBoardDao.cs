using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Core.Persistence {
    /// <summary>
    ///     Schnittstelle für Daos für <see cref="Board" />
    /// </summary>
    public interface IBoardDao : IGenericDao<Board, int> {
        /// <summary>
        ///     Ruft seitenweise archivierte Boards ab.
        ///     Die Boards werden sortiert nach <see cref="Board.ArchivedAt">Archivierungsdatum</see> abgerufen. Das zuletzt
        ///     archivierte Board wird als erstes gefunden.
        /// </summary>
        /// <param name="pageRequest">Seiteninformation</param>
        /// <param name="user">Der Nutzer, für den die Boards abgerufen werden sollen.</param>
        /// <returns>Die archivierten Boards</returns>
        IPage<Board> FindArchivedBoards(IPageable pageRequest, User user);

        /// <summary>
        ///     Liefert aller Boards, die der Nutzer verwenden darf, da sie entweder öffentlich sind oder der Nutzer Eigentümer
        ///     bzw. Mitglied des Boards ist.
        ///     Wird eine Suchzeichenfolge übergeben (mindestens 1 Zeichen ungleich Leerzeichen), werden nur Boards geliefert,
        ///     welche die angegebene Zeichenfolge im Namen tragen.
        /// </summary>
        /// <param name="pageRequest">Seiteninformationen für die Abfrage.</param>
        /// <param name="user"></param>
        /// <param name="searchTerm">Optionale Suchzeichenfolge, zur Einschränkung der gefundenen Boards.</param>
        /// <returns></returns>
        IPage<Board> FindBoardsForUser(IPageable pageRequest, User user, string searchTerm = null);

        /// <summary>
        ///     Ruft seitenweise Boards ab, denen ein bestimmtes Team zugewiesen ist.
        /// </summary>
        /// <param name="pageRequest">Seiteninformationen</param>
        /// <param name="team">Das Team, für welches die Boards abgerufen werden sollen, denen es zugewiesen ist.</param>
        /// <returns></returns>
        IPage<Board> FindBoardsWithTeam(IPageable pageRequest, Team team);

        /// <summary>
        ///     Ruft seitenweise Board-Vorlagen ab.
        /// </summary>
        /// <param name="pageRequest">Seiteninformation</param>
        /// <param name="user">Der Nutzer, für den die Vorlagen abgerufen werden sollen.</param>
        /// <returns>Die Board-Vorlagen</returns>
        IPage<Board> FindBoardTemplatesForUser(IPageable pageRequest, User user);
    }
}