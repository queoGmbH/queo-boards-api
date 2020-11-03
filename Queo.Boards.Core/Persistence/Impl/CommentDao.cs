using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Spring.Data.NHibernate.Generic;

namespace Queo.Boards.Core.Persistence.Impl {
    /// <summary>
    ///     Dao für <see cref="Comment" />
    /// </summary>
    public class CommentDao : GenericDao<Comment, int>, ICommentDao {
        /// <summary>
        ///     Liefert alle Kommentare einer Karte
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public IList<Comment> FindAllOnCard(Card card) {
            return Session.QueryOver<Comment>().Where(x => x.Card == card).List();
        }

        /// <summary>
        /// Sucht nach Kommentaren auf Karten, die für einen Nutzer zugänglich sind.
        /// </summary>
        /// <param name="pageRequest">Seiteninformationen für das Abrufen</param>
        /// <param name="user">Der Nutzer, für den die Kommentare abgerufen werden sollen.</param>
        /// <param name="searchTerm">Optionale Zeichenfolge, die ein Kommentar im <see cref="Comment.Text"/> enthalten muss, damit er gefunden wird.</param>
        /// <returns></returns>
        public IPage<Comment> FindCommentsForUser(IPageable pageRequest, User user, string searchTerm = null) {

            HibernateDelegate<IPage<Comment>> finder = delegate(ISession session) {

                /*Subquery für Boards, auf welcher der Nutzer zugreifen darf.*/
                QueryOver<Board, Board> boardsForUserSubquery = QueryOver.Of<Board>();
                /*Bei einer Suche sehen Admins alle Boards.*/
                BoardDao.AddUserCanFindBoardQueries(boardsForUserSubquery, user, !string.IsNullOrWhiteSpace(searchTerm));
                boardsForUserSubquery.WhereNot(board => board.IsArchived);
                boardsForUserSubquery.Select(board => board.Id);

                /*Subquery für relevante/zulässige Listen*/
                QueryOver<List, List> listsForUserSubquery = QueryOver.Of<List>();
                /*Nur Listen in für den Nutzer zugänglichen Boards.*/
                listsForUserSubquery.WithSubquery.WhereProperty(list => list.Board).In(boardsForUserSubquery).Select(list => list.Id);
                listsForUserSubquery.WhereNot(list => list.IsArchived);

                /*Nur Karten in zugelassenen Listen (und damit auch nur auf zugelassenen Boards)*/
                QueryOver<Card, Card> cardsForUserSubquery = QueryOver.Of<Card>();
                cardsForUserSubquery.WithSubquery.WhereProperty(card => card.List).In(listsForUserSubquery).Select(card => card.Id);
                cardsForUserSubquery.WhereNot(card => card.IsArchived);


                /*Die eigentliche Query zum Abrufen der Kommentare.*/
                IQueryOver<Comment, Comment> query = session.QueryOver<Comment>();

                /*Nur Kommentare in zugelassenen Karten (und damit auch nur auf zugelassenen Listen bzw. Boards)*/
                query.WithSubquery.WhereProperty(comment => comment.Card).In(cardsForUserSubquery);

                /*Keine gelöschten Kommentare*/
                query.WhereNot(card => card.IsDeleted);

                if (!string.IsNullOrWhiteSpace(searchTerm)) {
                    query.WhereRestrictionOn(card => card.Text).IsInsensitiveLike(searchTerm.Trim(), MatchMode.Anywhere);
                }

                return FindPage(query, pageRequest);

            };

            return HibernateTemplate.Execute(finder);
        }
    }
}