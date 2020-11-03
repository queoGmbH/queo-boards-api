using System;
using System.Collections.Generic;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Core.Persistence {
    /// <summary>
    ///     Schnittstelle für Daos für <see cref="Card" />
    /// </summary>
    public interface ICardDao : IGenericDao<Card, int> {

        /// <summary>
        ///     Liefert alle Karten mit einem bestimmten Label
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        IList<Card> FindAllCardsWithLabel(Label label);

        /// <summary>
        /// Sucht nach allen, nicht archivierten Karten
        /// </summary>
        /// <param name="pageRequest"></param>
        /// <param name="user"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        IPage<Card> FindCardsForUser(IPageable pageRequest, User user, string searchTerm = null);

        /// <summary>
        /// Sucht nach allen, nicht archivierten Karten
        /// </summary>
        /// <param name="board">Das Board, auf dem nach Karten gesucht werden soll, dem die Nutzer zugewiesen sind</param>
        /// <param name="users">Liste mit Nutzern, nach deren Karten gesucht werden soll.</param>
        /// <returns></returns>
        IList<Card> FindCardsOnBoardForUsers(Board board, params User[] users);


        /// <summary>
        /// Sucht nach Karten, deren Fälligkeit abgelaufen ist, für die aber noch keine Benachrichtigung darüber versendet wurde.
        /// 
        /// Es werden alle Karten geliefert, bei den gilt:
        ///  - <see cref="Card.Due"/> &lt; <see cref="dueExpirationLimit"/> 
        ///  - <see cref="Card.DueExpirationNotificationCreated"/> == false
        /// </summary>
        /// <param name="dueExpirationLimit">Das für die Prüfung des Ablaufs der Fälligkeit einer Karte verwendete Datum.</param>
        /// <returns></returns>
        IList<Card> FindCardsWithExpiredDueAndWithoutNotifications(DateTime dueExpirationLimit);
    }
}