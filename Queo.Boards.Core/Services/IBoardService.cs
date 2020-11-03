using System;
using System.Collections.Generic;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Core.Services {
    public interface IBoardService {
        /// <summary>
        ///     Fügt einen Nutzer einem Board hinzu.
        /// </summary>
        /// <param name="board">Das Board dem der Nutzer hinzugefügt werden soll.</param>
        /// <param name="userToAdd">Der zum Board hinzugefügte Nutzer.</param>
        /// <param name="addingUser">Der Nutzer, der den Nutzer zum Board hinzugefügt hat.</param>
        /// <returns>Liste aller dem Board zugewiesenen Nutzer.</returns>
        IList<User> AddMember(Board board, User userToAdd, User addingUser);

        /// <summary>
        ///     Fügt dem Board einen weiteren Besitzer hinzu.
        ///     Ist der Nutzer bereits Besitzer des Boards, wird keine Änderung vorgenommen.
        /// </summary>
        /// <param name="board">Das Board dem ein neuer Besitzer hinzugefügt werden soll.</param>
        /// <param name="userToBecomeOwner">Der Nutzer der als Besitzer des Boards hinzugefügt werden soll.</param>
        /// <returns>Liste der Nutzer, die aktuell als Besitzer des Boards definiert ist.</returns>
        IList<User> AddOwner(Board board, User userToBecomeOwner);

        /// <summary>
        ///     Weißt dem Board ein Team zu.
        ///     Wenn das Team dem Board bereits zugewiesen ist, werden keine Änderungen vorgenommen.
        /// </summary>
        /// <param name="board">Das Board, dem das Team zugewiesen werden soll</param>
        /// <param name="team">Das Team, welches dem Board zugewiesen werden soll</param>
        /// <param name="addingUser">Der Nutzer, der das Team zum Board hinzufügt</param>
        /// <returns></returns>
        IList<Team> AddTeam(Board board, Team team, User addingUser);

        /// <summary>
        ///     Erstellt eine Kopie eines Boards.
        ///     Das zu kopierende Board darf nicht archiviert sein.
        ///     Es kann jedoch <see cref="Board.IsTemplate">eine Vorlage</see> sein. Dann entspricht diese Methode einem Erstellen
        ///     eines Boards aus einer Vorlage.
        /// </summary>
        /// <param name="source">Das Board, welches kopiert werden soll.</param>
        /// <param name="boardDto"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        Board Copy(Board source, BoardDto boardDto, User createdBy);

        /// <summary>
        ///     Erstellt ein neues Board
        /// </summary>
        /// <param name="boardDto"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        Board Create(BoardDto boardDto, User creator);

        /// <summary>
        ///     Wandelt ein Board in ein Template um.
        ///     Dabei wird prinzipiell nur das Flag <see cref="Board.IsTemplate">IsTemplate</see> gesetzt.
        /// </summary>
        /// <param name="sourceBoard">Das Board, das zum Template umgewandelt werden soll</param>
        /// <param name="creator">Wer erstellt das Template.</param>
        /// <remarks>
        ///     Die Karten des umzuwandelnden Boards, werden nicht übernommen.
        ///     Die Besitzer, Teams und Member des umzuwandelnden Boards werden nicht übernommen.
        ///     TODO: Es sollen Aktivitäten entfernt werden. Gibt es die überhaupt schon?
        /// </remarks>
        /// <returns></returns>
        Board CreateTemplateFromBoard(Board sourceBoard, User creator);

        /// <summary>
        ///     Löscht ein Board endgültig aus der Datenbank.
        /// </summary>
        /// <param name="board"></param>
        void Delete(Board board);

        /// <summary>
        ///     Sucht nach allen Kommentaren, die an irgendeinem Objekt am Board getätigt wurden. Die Kommentare sind in
        ///     chronologischer Reihenfolge.
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        IList<Comment> FindAggregatedBoardComments(Board board);

        /// <summary>
        ///     Ruft seitenweise archivierte Boards ab.
        ///     Die Boards werden sortiert nach <see cref="Board.ArchivedAt">Archivierungsdatum</see> abgerufen. Das zuletzt
        ///     archivierte Board wird als erstes gefunden.
        /// </summary>
        /// <param name="pageRequest">Seiteninformation</param>
        /// <param name="user">Der Nutzer, für den die Boards abgerufen werden sollen.</param>
        /// <returns>Die archivierten Boards</returns>
        IPage<Board> FindArchivedBoardsForUser(IPageable pageRequest, User user);

        /// <summary>
        ///     Liefert aller Boards, die der Nutzer verwenden darf, da sie entweder öffentlich sind oder der Nutzer Eigentümer
        ///     bzw. Mitglied des Boards ist.
        ///     Wird eine Suchzeichenfolge übergeben, werden nur Boards geliefert, welche die angegebene Zeichenfolge im Namen
        ///     tragen.
        /// </summary>
        /// <param name="pageRequest">Seiteninformationen für die Suche</param>
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

        /// <summary>
        ///     Sucht nach einem Board mit der übergebenen BusinessId.
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        Board FindByBusinessId(Guid businessId);

        /// <summary>
        ///     Entfernt die Mitgliedschaft eines Nutzers am Board.
        ///     Außerdem wird der Nutzer von allen Karten des Boards entfernt, denen er zugewiesen war.
        /// </summary>
        /// <param name="board">Das Board</param>
        /// <param name="member">Der Nutzer, dessen Mitgliedschaft beendet werden soll.</param>
        /// <returns></returns>
        IList<User> RemoveMember(Board board, User member);

        /// <summary>
        ///     Entfernt einen Besitzer vom Board.
        ///     Ist der Nutzer kein Besitzer des Boards, wird keine Änderung vorgenommen.
        /// </summary>
        /// <param name="board">Das Board dem der Besitzer entfernt werden soll.</param>
        /// <param name="ownerToRemove">Der Nutzer der als Besitzer des Boards entfernt werden soll.</param>
        /// <returns>Liste der verbleibenden Besitzer des Boards.</returns>
        /// <exception cref="InvalidOperationException">
        ///     Es muss immer mindestens ein Besitzer am Board definiert sein.
        ///     Wenn der letzte Besitzer des Boards entfernt werden soll, wird eine <see cref="InvalidOperationException" />
        ///     geworfen.
        /// </exception>
        IList<User> RemoveOwner(Board board, User ownerToRemove);

        /// <summary>
        ///     Entfernt ein Team von einem Board.
        ///     Außerdem werden alle Nutzer, die dem Board nicht explizit als Mitglied oder Besitzer zugewiesen sind von allen
        ///     Karten entfernt.
        ///     Ist das zu entfernende Team dem Board nicht zugewiesen, werden keine Änderungen vorgenommen.
        /// </summary>
        /// <param name="board">Das Board, von welchem das Team entfernt werden soll</param>
        /// <param name="team">Das Team, welches vom Board entfernt werden soll.</param>
        /// <returns>Liste der verbleibenden Teams am Board.</returns>
        IList<Team> RemoveTeam(Board board, Team team);

        /// <summary>
        ///     Aktualisiert ein Board
        /// </summary>
        /// <param name="board"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        Board Update(Board board, BoardDto dto);

        /// <summary>
        ///     Aktualisiert ob ein Board archiviert ist.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="isArchived"></param>
        /// <returns></returns>
        Board UpdateIsArchived(Board board, bool isArchived);
    }
}