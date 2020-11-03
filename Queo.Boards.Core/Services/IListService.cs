using System;
using System.Collections.Generic;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Services {
    /// <summary>
    ///     Schnittstelle für Services für <see cref="List" />
    /// </summary>
    public interface IListService {
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
        List Copy(List source, Board targetBoard, string copyName, User copier, int position = 0);

        /// <summary>
        ///     Erstellt eine neue Liste
        /// </summary>
        /// <param name="board"></param>
        /// <param name="listName"></param>
        /// <returns></returns>
        List Create(Board board, string listName);

        /// <summary>
        ///     Ermittelt die Position, an der eine Liste eingefügt werden muss, um hinter einer bestimmten Liste auf einem Board
        ///     eingefügt zu werden.
        /// </summary>
        /// <param name="targetBoard">Das Board auf welcher die Liste eingefügt werden soll.</param>
        /// <param name="insertAfter">Die Liste hinter welche die neue Liste eingefügt werden soll.</param>
        /// <returns></returns>
        int GetPositionByTarget(Board targetBoard, List insertAfter);

        /// <summary>
        ///     Verschiebt eine Liste auf ein Board und optional hinter eine andere Liste
        /// </summary>
        /// <param name="listToMove"></param>
        /// <param name="targetBoard"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        Board MoveList(List listToMove, Board targetBoard, int position = 0);

        /// <summary>
        ///     Aktualisiert den Titel einer bestehenden Liste
        /// </summary>
        /// <param name="list"></param>
        /// <param name="listTitle"></param>
        /// <returns></returns>
        List Update(List list, string listTitle);

        /// <summary>
        ///     Aktualisiert ob eine Liste archiviert ist.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="isArchived"></param>
        /// <returns></returns>
        List UpdateArchived(List list, bool isArchived);

        /// <summary>
        /// Lädt alle <see cref="List"/> mit allen <see cref="Card"/> zu einer <see cref="Board"/>-BusinessId
        /// </summary>
        /// <param name="boardId">Die BusinessId des <see cref="Board"/> für das alles abgerufen werden soll</param>
        /// <returns></returns>
        IList<List> FindAllListsAndCardsByBoardId(Guid boardId);
    }
}