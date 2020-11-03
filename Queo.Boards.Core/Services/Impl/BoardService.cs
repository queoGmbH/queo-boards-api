using System;
using System.Collections.Generic;
using System.Linq;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Persistence;
using Spring.Transaction.Interceptor;

namespace Queo.Boards.Core.Services.Impl {
    /// <summary>
    ///     Service für <see cref="Board" />
    /// </summary>
    public class BoardService : IBoardService {
        private readonly IBoardDao _boardDao;
        private readonly ICardService _cardService;

        private readonly IEmailNotificationService _emailNotificationService;

        private readonly ILabelService _labelService;
        private readonly IListService _listService;

        /// <summary>
        /// </summary>
        /// <param name="boardDao"></param>
        /// <param name="labelService"></param>
        /// <param name="listService"></param>
        /// <param name="cardService"></param>
        /// <param name="emailNotificationService"></param>
        public BoardService(IBoardDao boardDao, ILabelService labelService, IListService listService, ICardService cardService,
            IEmailNotificationService emailNotificationService) {
            _boardDao = boardDao;
            _emailNotificationService = emailNotificationService;
            _labelService = labelService;
            _listService = listService;
            _cardService = cardService;
        }

        /// <summary>
        ///     Fügt einen Nutzer einem Board hinzu.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="userToAdd"></param>
        /// <param name="addingUser"></param>
        [Transaction]
        public IList<User> AddMember(Board board, User userToAdd, User addingUser) {
            Require.NotNull(userToAdd);
            Require.NotNull(board);
            ValidateCanEditBoard(board);
            ValidateIsNoTemplate(board);

            if (!board.Members.Contains(userToAdd)) {
                board.AddMember(userToAdd);
            }

            _emailNotificationService.NotifyUserAddedToBoard(userToAdd, board, addingUser);

            return board.Members.ToList();
        }

        /// <summary>
        ///     Fügt dem Board einen weiteren Besitzer hinzu.
        ///     Ist der Nutzer bereits Besitzer des Boards, wird keine Änderung vorgenommen.
        /// </summary>
        /// <param name="board">Das Board dem ein neuer Besitzer hinzugefügt werden soll.</param>
        /// <param name="userToBecomeOwner">Der Nutzer der als Besitzer des Boards hinzugefügt werden soll.</param>
        /// <returns>Liste der Nutzer, die aktuell als Besitzer des Boards definiert ist.</returns>
        [Transaction]
        public IList<User> AddOwner(Board board, User userToBecomeOwner) {
            Require.NotNull(board, "board");
            Require.NotNull(userToBecomeOwner, "userToBecomeOwner");
            ValidateCanEditBoard(board);
            ValidateIsNoTemplate(board);

            board.AddOwner(userToBecomeOwner);
            return board.Owners.ToList();
        }

        /// <summary>
        ///     Weißt dem Board ein Team zu.
        ///     Wenn das Team dem Board bereits zugewiesen ist, werden keine Änderungen vorgenommen.
        /// </summary>
        /// <param name="board">Das Board, dem das Team zugewiesen werden soll</param>
        /// <param name="team">Das Team, welches dem Board zugewiesen werden soll</param>
        /// <param name="addingUser">Der Nutzer, der das Team zum Board hinzufügt</param>
        /// <returns></returns>
        [Transaction]
        public IList<Team> AddTeam(Board board, Team team, User addingUser) {
            Require.NotNull(board, "board");
            Require.NotNull(team, "team");
            Require.NotNull(addingUser, "addingUser");

            ValidateCanEditBoard(board);
            ValidateIsNoTemplate(board);

            IList<User> existingBoardUsers = board.GetBoardUsers();

            board.AddTeam(team);

            foreach (User teamMember in team.Members.Except(existingBoardUsers)) {
                /*Nutzer, die nicht sowieso schon Mitglieder des Boards sind per E-Mail benachrichtigen*/
                _emailNotificationService.NotifyUserAddedToBoard(teamMember, board, addingUser);
            }

            return board.Teams.ToList();
        }

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
        [Transaction]
        public Board Copy(Board source, BoardDto boardDto, User createdBy) {
            Require.NotNull(source, "source");
            Require.NotNull(boardDto, "boardDto");
            Require.NotNull(createdBy, "createdBy");
            if (source.IsArchived) {
                throw new ArgumentOutOfRangeException("source", "Ein archiviertes Board darf nicht kopiert werden.");
            }

            Board copy = new Board(boardDto, new EntityCreatedDto(createdBy, DateTime.UtcNow), new List<User> {createdBy});
            _boardDao.Save(copy);

            foreach (Label sourceLabel in source.Labels) {
                /*Kopie der Labels übernehmen*/
                _labelService.Create(copy, sourceLabel.GetDto());
            }

            foreach (List sourceList in source.Lists.Where(l => !l.IsArchived)) {
                /*Kopie der Listen übernehmen*/
                /*Hinweis: Hier kein Create, damit auch Karten übernommen werden.*/
                _listService.Copy(sourceList, copy, sourceList.Title, createdBy, int.MaxValue);
            }

            return copy;
        }

        /// <summary>
        ///     Erstellt ein neues Board
        /// </summary>
        /// <param name="boardDto"></param>
        /// <param name="creator">Der Ersteller des Boards, der gleichzeitig auch Eigentümer des Boards wird.</param>
        /// <returns></returns>
        [Transaction]
        public Board Create(BoardDto boardDto, User creator) {
            Require.NotNull(boardDto);
            Require.NotNull(creator);
            Board board = new Board(boardDto, new EntityCreatedDto(creator, DateTime.Now.ToUniversalTime()), new List<User> {creator});
            _boardDao.Save(board);
            return board;
        }

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
        [Transaction]
        public Board CreateTemplateFromBoard(Board sourceBoard, User creator) {
            Require.NotNull(sourceBoard, "sourceBoard");
            if (sourceBoard.IsArchived) {
                throw new ArgumentOutOfRangeException("sourceBoard", "Aus einem archivierten Board kann keine Vorlage erstellt werden!");
            }

            if (sourceBoard.IsTemplate) {
                throw new ArgumentOutOfRangeException("sourceBoard", "Aus einer Vorlage kann keine neue Vorlage erstellt werden!");
            }

            Board template = new Board(new BoardDto(sourceBoard.Title, sourceBoard.Accessibility, sourceBoard.ColorScheme),
                new EntityCreatedDto(creator, DateTime.UtcNow), true);
            _boardDao.Save(template);

            /*Labels als Kopie übernehmen*/
            foreach (Label sourceBoardLabel in sourceBoard.Labels) {
                _labelService.Create(template, sourceBoardLabel.GetDto());
            }

            /*Listen übernehmen, aber ohne Karten*/
            foreach (List sourceBoardList in sourceBoard.Lists) {
                _listService.Create(template, sourceBoardList.Title);
            }

            return template;
        }

        /// <summary>
        ///     Löscht ein Board endgültig aus der Datenbank.
        /// </summary>
        /// <param name="board"></param>
        [Transaction]
        public void Delete(Board board) {
            Require.NotNull(board, "board");

            _boardDao.Delete(board);
        }

        /// <summary>
        ///     Sucht nach allen Kommentaren, die an irgendeinem Objekt am Board getätigt wurden. Die Kommentare sind in
        ///     chronologischer Reihenfolge.
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public IList<Comment> FindAggregatedBoardComments(Board board) {
            Require.NotNull(board, "board");

            List<Comment> comments = new List<Comment>();
            comments.AddRange(board.Lists.Where(l => l != null && l.IsArchived == false)
                .SelectMany(l => l.Cards.Where(c => c != null && c.IsArchived == false).SelectMany(c => c.Comments)));

            return comments.OrderBy(c => c.CreationDate).ToList();
        }

        /// <summary>
        ///     Ruft seitenweise archivierte Boards ab.
        ///     Die Boards werden sortiert nach <see cref="Board.ArchivedAt">Archivierungsdatum</see> abgerufen. Das zuletzt
        ///     archivierte Board wird als erstes gefunden.
        /// </summary>
        /// <param name="pageRequest">Seiteninformation</param>
        /// <param name="user">Der Nutzer, für den die Boards abgerufen werden sollen.</param>
        /// <returns>Die archivierten Boards</returns>
        public IPage<Board> FindArchivedBoardsForUser(IPageable pageRequest, User user) {
            return _boardDao.FindArchivedBoards(pageRequest, user);
        }

        /// <summary>
        ///     Liefert aller Boards, die der Nutzer verwenden darf, da sie entweder öffentlich sind oder der Nutzer Eigentümer
        ///     bzw. Mitglied des Boards ist.
        ///     Wird eine Suchzeichenfolge übergeben, werden nur Boards geliefert, welche die angegebene Zeichenfolge im Namen
        ///     tragen.
        /// </summary>
        /// <param name="pageRequest"></param>
        /// <param name="user"></param>
        /// <param name="searchTerm">Optionale Suchzeichenfolge, zur Einschränkung der gefundenen Boards.</param>
        /// <returns></returns>
        public IPage<Board> FindBoardsForUser(IPageable pageRequest, User user, string searchTerm = null) {
            return _boardDao.FindBoardsForUser(pageRequest, user, searchTerm);
        }

        /// <summary>
        ///     Ruft seitenweise Boards ab, denen ein bestimmtes Team zugewiesen ist.
        /// </summary>
        /// <param name="pageRequest">Seiteninformationen</param>
        /// <param name="team">Das Team, für welches die Boards abgerufen werden sollen, denen es zugewiesen ist.</param>
        /// <returns></returns>
        public IPage<Board> FindBoardsWithTeam(IPageable pageRequest, Team team) {
            return _boardDao.FindBoardsWithTeam(pageRequest, team);
        }

        /// <summary>
        ///     Ruft seitenweise Board-Vorlagen ab.
        /// </summary>
        /// <param name="pageRequest">Seiteninformation</param>
        /// <param name="user">Der Nutzer, für den die Vorlagen abgerufen werden sollen.</param>
        /// <returns>Die Board-Vorlagen</returns>
        public IPage<Board> FindBoardTemplatesForUser(IPageable pageRequest, User user) {
            return _boardDao.FindBoardTemplatesForUser(pageRequest, user);
        }

        /// <summary>
        ///     Sucht nach einem Board mit der übergebenen BusinessId.
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        public Board FindByBusinessId(Guid businessId) {
            return _boardDao.GetByBusinessId(businessId);
        }

        /// <summary>
        ///     Entfernt die Mitgliedschaft eines Nutzers am Board.
        ///     Außerdem wird der Nutzer von allen Karten des Boards entfernt, denen er zugewiesen war.
        /// </summary>
        /// <param name="board">Das Board</param>
        /// <param name="member">Der Nutzer, dessen Mitgliedschaft beendet werden soll.</param>
        /// <returns></returns>
        [Transaction]
        public IList<User> RemoveMember(Board board, User member) {
            Require.NotNull(board, "board");
            Require.NotNull(member, "member");
            ValidateCanEditBoard(board);
            ValidateIsNoTemplate(board);

            /*Nutzer von Board entfernen*/
            board.RemoveMember(member);

            /*Nutzer von allen Karten entfernen, denen er zugewiesen ist*/
            IList<Card> cardsAssignedToRemovedMember =
                board.Lists.SelectMany(list => list.Cards).Where(card => card.AssignedUsers.Contains(member)).ToList();
            foreach (Card card in cardsAssignedToRemovedMember) {
                card.UnassignUser(member);
            }

            return board.Members.ToList();
        }

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
        [Transaction]
        public IList<User> RemoveOwner(Board board, User ownerToRemove) {
            Require.NotNull(board, "board");
            Require.NotNull(ownerToRemove, "ownerToRemove");
            ValidateCanEditBoard(board);
            ValidateIsNoTemplate(board);

            if (board.Owners.All(o => o.Equals(ownerToRemove))) {
                throw new InvalidOperationException("Es muss mindestens ein Nutzer als Besitzer des Boards festgelegt sein.");
            }

            board.RemoveOwner(ownerToRemove);
            return board.Owners.ToList();
        }

        /// <summary>
        ///     Entfernt ein Team von einem Board.
        ///     Außerdem werden alle Nutzer, die dem Board nicht explizit als Mitglied oder Besitzer zugewiesen sind von allen
        ///     Karten entfernt.
        ///     Ist das zu entfernende Team dem Board nicht zugewiesen, werden keine Änderungen vorgenommen.
        /// </summary>
        /// <param name="board">Das Board, von welchem das Team entfernt werden soll</param>
        /// <param name="team">Das Team, welches vom Board entfernt werden soll.</param>
        /// <returns>Liste der verbleibenden Teams am Board.</returns>
        [Transaction]
        public IList<Team> RemoveTeam(Board board, Team team) {
            Require.NotNull(board, "board");
            Require.NotNull(team, "team");

            ValidateCanEditBoard(board);
            ValidateIsNoTemplate(board);

            board.RemoveTeam(team);

            IList<User> remainingBoardUsers = board.GetBoardUsers();

            IList<Card> cards = board.Lists.SelectMany(l => l.Cards).ToList();
            User[] deletedUsers = team.Members.Except(remainingBoardUsers).ToArray();
            if (deletedUsers.Any()) {
                foreach (Card card in cards) {
                    _cardService.UnassignUsers(card, deletedUsers);
                }
            }

            return board.Teams.ToList();
        }

        /// <summary>
        ///     Aktualisiert ein Board
        /// </summary>
        /// <param name="board"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Transaction]
        public Board Update(Board board, BoardDto dto) {
            Require.NotNull(board);
            ValidateCanEditBoard(board);

            board.Update(dto);
            return board;
        }

        /// <summary>
        ///     Aktualisiert ob ein Board archiviert ist.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="isArchived"></param>
        /// <returns></returns>
        [Transaction]
        public Board UpdateIsArchived(Board board, bool isArchived) {
            Require.NotNull(board, "board");

            ValidateIsNoTemplate(board);

            if (isArchived) {
                board.Archive(DateTime.UtcNow);
            } else {
                board.Restore();
            }

            return board;
        }

        /// <summary>
        ///     Überprüft, ob ein Board geändert werden darf.
        /// </summary>
        /// <param name="board"></param>
        public static void ValidateCanEditBoard(Board board) {
            Require.NotNull(board, "board");

            if (board.IsArchived) {
                throw new ArgumentOutOfRangeException("board", "Ein archiviertes Board kann nicht geändert werden.");
            }
        }

        /// <summary>
        ///     Überprüft, dass es sich bei dem Board nicht um eine Vorlage handelt.
        /// </summary>
        /// <param name="board">das zu überprüfende Board</param>
        public static void ValidateIsNoTemplate(Board board) {
            if (board.IsTemplate) {
                throw new ArgumentOutOfRangeException("board", "Die Aktion ist bei Board-Vorlagen nicht möglich.");
            }
        }
    }
}