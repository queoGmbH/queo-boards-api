using System;
using System.Collections.Generic;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Core.Services {
    /// <summary>
    ///     Schnittstelle für Services für <see cref="Card" />
    /// </summary>
    public interface ICardService {
        /// <summary>
        ///     Fügt einer Karte ein Label hinzu.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="labelBusinessId"></param>
        Card AddLabel(Card card, Label labelBusinessId);

        /// <summary>
        ///     Weißt der Karte einen Nutzer zu.
        /// </summary>
        /// <param name="card">Die Karte, welcher der Nutzer hinzugefügt werden soll.</param>
        /// <param name="user">Der hinzuzufügende Nutzer</param>
        /// <returns></returns>
        IList<User> AssignUser(Card card, User user);

        /// <summary>
        ///     Kopiert eine Karte und speichert die Kopie auf der Liste und an der entsprechenden Position bzw. am Ende der Liste.
        ///     Es werden alle Eigenschaften außer Anhängen übernommen.
        /// </summary>
        /// <param name="sourceCard"></param>
        /// <param name="targetList"></param>
        /// <param name="copyName">Der Name der zu erstellenden Kopie</param>
        /// <param name="copier">Der kopierende Nutzer und Ersteller der Kopie</param>
        /// <param name="position"></param>
        /// <returns></returns>
        Card Copy(Card sourceCard, List targetList, string copyName, User copier, int position = 0);

        /// <summary>
        ///     Erstellt eine neue Karte
        /// </summary>
        /// <param name="list"></param>
        /// <param name="dto"></param>
        /// <param name="initiallyAssignedUsers">Liste der Initial der Karte zugeordneten Nutzer.</param>
        /// <param name="creator">Der Ersteller der Karte</param>
        /// <returns></returns>
        Card Create(List list, CardDto dto, IList<User> initiallyAssignedUsers, User creator);

        /// <summary>
        ///     Ermittelt die Position, an der eine Karte eingefügt werden muss, um hinter einer bestimmten Karte auf einer Liste
        ///     eingefügt zu werden.
        /// </summary>
        /// <param name="targetList">Die Liste auf welcher die Karte eingefügt werden soll.</param>
        /// <param name="insertAfter">Die Karte hinter welche die neue Liste eingefügt werden soll.</param>
        /// <returns></returns>
        int GetPositionByTarget(List targetList, Card insertAfter);

        /// <summary>
        ///     Verschiebt eine Karte zwischen Listen
        /// </summary>
        /// <param name="cardToMove">Die zu verschiebende Karte</param>
        /// <param name="targetList">Die Zielliste</param>
        /// <param name="position">
        ///     Der Index an welcher die Karte auf der Zielliste eingefügt werden soll.
        /// </param>
        /// <returns></returns>
        List MoveCard(Card cardToMove, List targetList, int position = 0);

        /// <summary>
        ///     Entfernt das Label von einer Karte
        /// </summary>
        /// <param name="card"></param>
        /// <param name="label"></param>
        Card RemoveLabel(Card card, Label label);

        /// <summary>
        ///     Aktualisiert ob eine Karte archiviert ist.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="isArchived"></param>
        /// <returns></returns>
        Card UpdateArchived(Card card, bool isArchived);

        /// <summary>
        ///     Aktualisiert die Beschreibung einer Karte
        /// </summary>
        /// <param name="card"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        Card UpdateDescription(Card card, string description);

        /// <summary>
        ///     Aktualisiert das Fälligkeitsdatum
        /// </summary>
        /// <param name="card"></param>
        /// <param name="due"></param>
        /// <returns></returns>
        Card UpdateDue(Card card, DateTime? due);

        /// <summary>
        ///     Aktualisiert die Karte
        /// </summary>
        /// <param name="card"></param>
        /// <param name="title"></param>
        Card UpdateTitle(Card card, string title);

        /// <summary>
        /// Sucht nach allen, nicht archivierten Karten
        /// </summary>
        /// <param name="pageRequest">Seiteninformationen für die Abfrage.</param>
        /// <param name="user">Der Nutzer, für den die Karten abgerufen werden sollen.</param>
        /// <param name="searchTerm">Optionale Suchzeichenfolge, zur Einschränkung der Ergebnis-Liste</param>
        /// <returns></returns>
        IPage<Card> FindCardsForUser(IPageable pageRequest, User user, string searchTerm = null);

        /// <summary>
        /// Entfernt einen Nutzer aus der Liste der einer Karte zugewiesenen Nutzer.
        /// </summary>
        /// <param name="card">Die Karte, von welcher der Nutzer entfernt werden sollen.</param>
        /// <param name="users">Die Nutzer, deren Zuordnung entfernt werden soll.</param>
        /// <returns>Die Liste der nach dem entfernen noch der Karte zugewiesenen Nutzer.</returns>
        IList<User> UnassignUsers(Card card, params User[] users);


        /// <summary>
        /// Sucht nach allen, nicht archivierten Karten
        /// </summary>
        /// <param name="board">Das Board, auf dem nach Karten gesucht werden soll, dem die Nutzer zugewiesen sind</param>
        /// <param name="users">Liste mit Nutzern, nach deren Karten gesucht werden soll.</param>
        /// <returns></returns>
        IList<Card> FindCardsOnBoardForUsers(Board board, params User[] users);
    }
}