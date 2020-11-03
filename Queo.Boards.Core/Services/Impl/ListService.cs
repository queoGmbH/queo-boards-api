using System;
using System.Collections.Generic;
using System.Linq;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Persistence;
using Spring.Transaction.Interceptor;

namespace Queo.Boards.Core.Services.Impl {
    /// <summary>
    ///     Service für <see cref="List" />
    /// </summary>
    public class ListService : IListService {
        private readonly ICardDao _cardDao;

        private readonly ICardService _cardService;
        private readonly IListDao _listDao;

        /// <summary>
        /// </summary>
        /// <param name="listDao"></param>
        /// <param name="cardDao"></param>
        /// <param name="cardService"></param>
        public ListService(IListDao listDao, ICardDao cardDao, ICardService cardService) {
            _listDao = listDao;
            _cardDao = cardDao;
            _cardService = cardService;
        }

        /// <summary>
        ///     Kopiert eine Liste. Dabei werden auch Kopien eines Großteils der darauf enthaltenen Elemente erstellt.
        ///     Folgende Elemente der Liste werden kopiert:
        /// </summary>
        /// <param name="source">Die zu kopierende Liste.</param>
        /// <param name="targetBoard">Das Board, auf welches die Liste kopiert werden soll.</param>
        /// <param name="copyName">Der Name der Kopie (neu erstellten Liste)</param>
        /// <param name="copier"></param>
        /// <param name="position">
        ///     Die Position auf dem Board, an der die Liste eingefügt werden soll. Wenn nicht angegeben, wird
        ///     die Liste am Ende des Board eingefügt.
        /// </param>
        /// <returns></returns>
        [Transaction]
        public List Copy(List source, Board targetBoard, string copyName, User copier, int position = 0) {
            Require.NotNull(targetBoard, "targetBoard");
            Require.NotNull(source, "source");
            Require.NotNull(copier, "copier");
            if (source.IsArchived) {
                throw new InvalidOperationException("Eine archivierte Liste kann nicht kopiert werden.");
            }

            if (targetBoard.IsTemplate) {
                throw new InvalidOperationException("Eine Liste kann nicht auf eine Vorlage kopiert werden.");
            }

            BoardService.ValidateCanEditBoard(targetBoard);

            List copy = new List(targetBoard, copyName);
            position = NormalizePosition(targetBoard, position);
            targetBoard.Lists.Insert(position, copy);

            _listDao.Save(copy);

            /*Karten kopieren*/
            foreach (Card sourceCard in source.Cards.Where(card => !card.IsArchived)) {
                _cardService.Copy(sourceCard, copy, sourceCard.Title, copier, copy.Cards.Count);
            }

            return copy;
        }

        /// <summary>
        ///     Erstellt eine neue Liste
        /// </summary>
        /// <param name="board"></param>
        /// <param name="listName"></param>
        /// <returns></returns>
        [Transaction]
        public List Create(Board board, string listName) {
            Require.NotNull(board, nameof(board));
            Require.NotNull(listName, nameof(listName));
            BoardService.ValidateCanEditBoard(board);

            int listPosition = CalculateNewListPosition(board);

            List list = new List(board, listName);
            board.Lists.Insert(listPosition, list);
            _listDao.Save(list);

            return list;
        }

        /// <summary>
        /// Lädt alle <see cref="List"/> mit allen <see cref="Card"/> zu einer <see cref="Board"/>-BusinessId
        /// </summary>
        /// <param name="boardId">Die BusinessId des <see cref="Board"/> für das alles abgerufen werden soll</param>
        /// <returns></returns>
        public IList<List> FindAllListsAndCardsByBoardId(Guid boardId) {
            Require.NotNull(boardId, nameof(boardId));

            return _listDao.FindAllListsAndCardsByBoardId(boardId);
        }

        /// <summary>
        ///     Ermittelt die Position, an der eine Liste eingefügt werden muss, um hinter einer bestimmten Liste auf einem Board
        ///     eingefügt zu werden.
        /// </summary>
        /// <param name="targetBoard">Das Board auf welcher die Liste eingefügt werden soll.</param>
        /// <param name="insertAfter">Die Liste hinter welche die neue Liste eingefügt werden soll.</param>
        /// <returns></returns>
        public int GetPositionByTarget(Board targetBoard, List insertAfter) {
            Require.NotNull(targetBoard, "targetBoard");

            if (insertAfter == null) {
                return 0;
            }

            return NormalizePosition(targetBoard, targetBoard.Lists.IndexOf(insertAfter) + 1);
        }

        /// <summary>
        ///     Verschiebt eine Liste auf ein Board und optional hinter eine andere Liste
        /// </summary>
        /// <param name="listToMove"></param>
        /// <param name="targetBoard"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        [Transaction]
        public Board MoveList(List listToMove, Board targetBoard, int position = 0) {
            Require.NotNull(listToMove);
            Require.NotNull(targetBoard);
            ValidateCanEditList(listToMove);
            if (targetBoard.IsTemplate) {
                throw new InvalidOperationException("Eine Liste kann nicht auf eine Vorlage verschoben werden.");
            }

            BoardService.ValidateCanEditBoard(targetBoard);

            Board sourceBoard = listToMove.Board;
            sourceBoard.Lists.Remove(listToMove);
            listToMove.UpdateParent(targetBoard);
            position = NormalizePosition(targetBoard, position);
            targetBoard.Lists.Insert(position, listToMove);

            if (!sourceBoard.Equals(targetBoard)) {
                foreach (Card card in listToMove.Cards) {
                    card.Labels.Clear();
                    card.ClearAssignedUsers();
                }
            }

            return targetBoard;
        }

        /// <summary>
        ///     Aktualisiert den Titel einer bestehenden Liste
        /// </summary>
        /// <param name="list"></param>
        /// <param name="listTitle"></param>
        /// <returns></returns>
        [Transaction]
        public List Update(List list, string listTitle) {
            Require.NotNull(list);
            Require.NotNull(listTitle);
            ValidateCanEditList(list);

            list.Update(listTitle);
            return list;
        }

        /// <summary>
        ///     Aktualisiert ob eine Liste archiviert ist.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="isArchived"></param>
        /// <returns></returns>
        [Transaction]
        public List UpdateArchived(List list, bool isArchived) {
            Require.NotNull(list, "list");

            if (isArchived) {
                list.Archive(DateTime.UtcNow);
            } else {
                list.Restore();
            }

            return list;
        }

        /// <summary>
        ///     Überprüft, ob eine Liste geändert werden kann.
        /// </summary>
        /// <param name="list"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValidateCanEditList(List list) {
            Require.NotNull(list, "list");

            if (list.IsArchived) {
                throw new ArgumentOutOfRangeException("list", "Eine archivierte Karte kann nicht geändert werden.");
            }

            BoardService.ValidateCanEditBoard(list.Board);
        }

        private int CalculateNewListPosition(Board board) {
            return board.Lists.Count;
        }

        private int NormalizePosition(Board targetBoard, int position) {
            /*Die Position darf nicht kleiner als 0 sein.*/
            position = Math.Max(0, position);
            /*Die Position darf nicht größer als die Anzahl der Listen auf dem Board sein.*/
            position = Math.Min(position, targetBoard.Lists.Count);

            return position;
        }
    }
}