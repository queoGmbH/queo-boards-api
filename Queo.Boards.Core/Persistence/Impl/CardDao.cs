using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Spring.Data.NHibernate.Generic;

namespace Queo.Boards.Core.Persistence.Impl {
    /// <summary>
    ///     Dao für <see cref="Card" />
    /// </summary>
    public class CardDao : GenericDao<Card, int>, ICardDao {

        /// <summary>
        ///     Liefert alle Karten mit einem bestimmten Label
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public IList<Card> FindAllCardsWithLabel(Label label) {
            FindHibernateDelegate<Card> finder = delegate (ISession session) {
                Label labelAlias = null;
                return
                        session.QueryOver<Card>()
                                .JoinAlias(x => x.Labels, () => labelAlias, JoinType.InnerJoin)
                                .Where(x => labelAlias.BusinessId == label.BusinessId)
                                
                                .List();
            };
            
            return HibernateTemplate.ExecuteFind(finder);
        }

        /// <summary>
        /// Sucht nach allen, nicht archivierten Karten
        /// </summary>
        /// <param name="pageRequest"></param>
        /// <param name="user"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public IPage<Card> FindCardsForUser(IPageable pageRequest, User user, string searchTerm = null) {


            HibernateDelegate<IPage<Card>> finder = delegate(ISession session) {

                /*Subquery für relevante/zugelassene Boards erstellen.*/
                QueryOver<Board, Board> boardsForUserSubquery = QueryOver.Of<Board>();

                /*Nur für den Nutzer zugängliche Boards. Bei einer Suche sehen Admins alle Boards*/
                BoardDao.AddUserCanFindBoardQueries(boardsForUserSubquery, user, !string.IsNullOrWhiteSpace(searchTerm));
                boardsForUserSubquery.WhereNot(board => board.IsArchived);
                boardsForUserSubquery.Select(board => board.Id);
                
                /*Subquery für relevante/zulässige Listen*/
                QueryOver<List, List> listsForUserSubquery = QueryOver.Of<List>();
                /*Nur Listen in für den Nutzer zugänglichen Boards.*/
                listsForUserSubquery.WithSubquery.WhereProperty(list => list.Board).In(boardsForUserSubquery).Select(list => list.Id);
                listsForUserSubquery.WhereNot(list => list.IsArchived);


                /*Die eigentliche Query nach den Karten.*/
                IQueryOver<Card, Card> query = session.QueryOver<Card>();

                /*Nur Karten in zugelassenen Listen (und damit auch nur auf zugelassenen Boards)*/
                query.WithSubquery.WhereProperty(card => card.List).In(listsForUserSubquery);
                query.WhereNot(card => card.IsArchived);

                if (!string.IsNullOrWhiteSpace(searchTerm)) {
                    Disjunction disjunction = new Disjunction();
                    disjunction.Add(Restrictions.On<Card>(card => card.Title).IsLike(searchTerm.Trim(), MatchMode.Anywhere));
                    disjunction.Add(Restrictions.On<Card>(card => card.Description).IsLike(searchTerm.Trim(), MatchMode.Anywhere));

                    query.And(disjunction);
                }

                /*Ergebnisseite abrufen*/
                return FindPage(query, pageRequest);
            };


            return HibernateTemplate.Execute(finder);
        }

        /// <summary>
        /// Sucht nach allen, nicht archivierten Karten
        /// </summary>
        /// <param name="board">Das Board, auf dem nach Karten gesucht werden soll, dem die Nutzer zugewiesen sind</param>
        /// <param name="users">Liste mit Nutzern, nach deren Karten gesucht werden soll.</param>
        /// <returns></returns>
        public IList<Card> FindCardsOnBoardForUsers(Board board, params User[] users) {
            Require.NotNull(board, "board");

            /*Zunächst alle nicht archivierten Karten an nicht archivierten Listen abrufen*/
            IList<Card> activeCards = board.Lists.Where(list => !list.IsArchived).SelectMany(list => list.Cards).Where(card => !card.IsArchived).ToList();

            if (users != null && users.Any()) {
                /*Wenn Einschränkung der Nutzer, dann nur Karten, wo mindestens einer der Nutzer zugewiesen ist*/
                return activeCards.Where(card => !card.IsArchived && card.AssignedUsers.Intersect(users).Any()).ToList();
            } else {
                return activeCards;
            }
        }

        /// <summary>
        /// Sucht nach Karten, deren Fälligkeit abgelaufen ist, für die aber noch keine Benachrichtigung darüber versendet wurde.
        /// 
        /// Es werden alle Karten geliefert, bei den gilt:
        ///  - <see cref="Card.Due"/> &lt; <see cref="dueExpirationLimit"/> 
        ///  - <see cref="Card.DueExpirationNotificationCreated"/> == false
        /// </summary>
        /// <param name="dueExpirationLimit">Das für die Prüfung des Ablaufs der Fälligkeit einer Karte verwendete Datum.</param>
        /// <returns></returns>
        public IList<Card> FindCardsWithExpiredDueAndWithoutNotifications(DateTime dueExpirationLimit) {

            dueExpirationLimit = dueExpirationLimit.ToUniversalTime();

            FindHibernateDelegate<Card> finder = delegate (ISession session) {
                return
                        session.QueryOver<Card>()
                            .WhereNot(card => card.IsArchived)
                            .Where(card => card.Due < dueExpirationLimit)
                            .AndNot(card => card.DueExpirationNotificationCreated)
                            .JoinQueryOver(card => card.List).WhereNot(list => list.IsArchived)
                            .JoinQueryOver(list => list.Board).WhereNot(board => board.IsArchived)
                            .List();
            };

            return HibernateTemplate.ExecuteFind(finder);
        }


    }
}