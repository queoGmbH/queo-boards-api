using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Spring.Data.NHibernate.Generic;

namespace Queo.Boards.Core.Persistence.Impl {
    /// <summary>
    /// Dao, der Methoden für die Persistierung von <see cref="Board"/>s anbietet.
    /// </summary>
    public class BoardDao : GenericDao<Board, int>, IBoardDao {
        /// <summary>
        ///     Fügt eine Query, die Boards abfragt, die Bedingungen hinzu, die erfüllt sein müssen, damit ein Nutzer das Board
        ///     finden darf.
        /// </summary>
        /// <param name="queryOver">Die Abfrage, welcher die Kriterien/Bedingungen hinzugefügt werden sollen.</param>
        /// <param name="user">Der Nutzer, für den die Bedingungen hinzugefügt werden sollen.</param>
        /// <param name="allBoardsForAdmin">Sollen wenn der Nutzer Admin ist alle Board beachtet werden?</param>
        public static void AddUserCanFindBoardQueries(IQueryOver<Board, Board> queryOver, User user, bool allBoardsForAdmin) {
            Require.NotNull(queryOver, "queryOver");

            if (allBoardsForAdmin && user.Roles.Contains(UserRole.ADMINISTRATOR)) {
                /*Admins dürfen alle Boards sehen.*/
                return;
            }

            if (user != null) {
                
                User member = null;
                User owner = null;

                Team team = null;
                User teamMember = null;

                QueryOver<Team, Team> usersTeams = QueryOver.Of<Team>().JoinAlias(t => t.Members, () => teamMember, JoinType.RightOuterJoin).Where(m => teamMember.Id == user.Id).Select(t => t.Id);

                QueryOver<Board, Board> userIsMemberOfBoardsSubquery = QueryOver.Of<Board>().JoinAlias(x => x.Members, () => member, JoinType.RightOuterJoin).Where(b => member.Id == user.Id).Select(board => board.Id);
                QueryOver<Board, Board> userIsMemberOfBoardViaTeamSubquery = QueryOver.Of<Board>().JoinAlias(x => x.Teams, () => team).WithSubquery.WhereProperty(b => team.Id).In(usersTeams).Select(board => board.Id);
                QueryOver<Board, Board> userIsOwnerOfBoardsSubquery = QueryOver.Of<Board>().JoinAlias(x => x.Owners, () => owner, JoinType.RightOuterJoin).Where(b => owner.Id == user.Id).Select(board => board.Id);
                QueryOver<Board, Board> publicBoardsSubquery = QueryOver.Of<Board>().Where(x => x.Accessibility == Accessibility.Public).Select(b => b.Id);

                Disjunction memberOrOwnerOrPublic = new Disjunction();
                memberOrOwnerOrPublic
                        /*Der Nutzer ist Eigentümer des Boards*/
                        .Add(Subqueries.WhereProperty<Board>(board => board.Id).In(userIsOwnerOfBoardsSubquery))
                        /*Der Nutzer ist Mitglied des Boards*/
                        .Add(Subqueries.WhereProperty<Board>(board => board.Id).In(userIsMemberOfBoardsSubquery))
                        /*Der Nutzer ist Mitglied eines Teams, das dem Board zugeordnet ist*/
                        .Add(Subqueries.WhereProperty<Board>(board => board.Id).In(userIsMemberOfBoardViaTeamSubquery))
                        /*Öffentliches Board*/
                        .Add(Subqueries.WhereProperty<Board>(board => board.Id).In(publicBoardsSubquery));
                
                queryOver.And(memberOrOwnerOrPublic);
            }
        }

        /// <summary>
        ///     Liefert aller Boards, die der Nutzer verwenden darf, da sie entweder öffentlich sind oder der Nutzer Eigentümer
        ///     bzw. Mitglied des Boards ist.
        ///     Wird eine Suchzeichenfolge übergeben (mindestens 1 Zeichen ungleich Leerzeichen), werden nur Boards geliefert,
        ///     welche die angegebene Zeichenfolge im Namen tragen.
        /// </summary>
        /// <param name="pageRequest">Seiteninformationen für die Abfrage</param>
        /// <param name="user"></param>
        /// <param name="searchTerm">Optionale Suchzeichenfolge, zur Einschränkung der gefundenen Boards.</param>
        /// <returns></returns>
        public IPage<Board> FindBoardsForUser(IPageable pageRequest, User user, string searchTerm = null) {
            Require.NotNull(pageRequest, "pageRequest");
            Require.NotNull(user, "user");

            HibernateDelegate<IPage<Board>> finder = delegate(ISession session) {
                IQueryOver<Board, Board> queryOver = session.QueryOver<Board>()
                .WhereNot(board => board.IsArchived)
                .AndNot(board => board.IsTemplate);

                /*Wenn nach Boards gesucht wird, dann darf der Admin alle Boards sehen*/
                AddUserCanFindBoardQueries(queryOver, user, !string.IsNullOrWhiteSpace(searchTerm));
                AddFindBoardBySearchTermQueries(queryOver, searchTerm);
                return FindPage(queryOver, pageRequest);
            };

            return HibernateTemplate.Execute(finder);
        }

        private static void AddFindBoardBySearchTermQueries(IQueryOver<Board, Board> queryOver, string searchTerm) {
            Require.NotNull(queryOver, "queryOver");

            if (!string.IsNullOrWhiteSpace(searchTerm)) {
                /*Das Board enthält die Suchzeichenfolge irgendwo im Namen*/
                queryOver.AndRestrictionOn(b => b.Title).IsLike(searchTerm.Trim(), MatchMode.Anywhere);
            }
        }

        /// <summary>
        /// Ruft seitenweise archivierte Boards ab.
        /// Die Boards werden sortiert nach <see cref="Board.ArchivedAt">Archivierungsdatum</see> abgerufen. Das zuletzt archivierte Board wird als erstes gefunden.
        /// </summary>
        /// <param name="pageRequest">Seiteninformation</param>
        /// <param name="user">Der Nutzer, für den die Boards abgerufen werden sollen.</param>
        /// <returns></returns>
        public IPage<Board> FindArchivedBoards(IPageable pageRequest, User user) {
            Require.NotNull(pageRequest, "pageRequest");
            
            HibernateDelegate<IPage<Board>> finder = delegate (ISession session) {

                User owner = null;
                QueryOver<Board, Board> userIsOwnerOfBoardsSubquery = QueryOver.Of<Board>().JoinAlias(x => x.Owners, () => owner, JoinType.RightOuterJoin).Where(b => owner.Id == user.Id).Select(board => board.Id);


                IQueryOver<Board, Board> queryOver = session.QueryOver<Board>()
                    .Where(board => board.IsArchived)
                    .AndNot(board => board.IsTemplate)
                    .OrderBy(board => board.ArchivedAt).Desc;

                if (!user.Roles.Contains(UserRole.ADMINISTRATOR)) {
                    queryOver.WithSubquery.WhereProperty(board => board.Id).In(userIsOwnerOfBoardsSubquery);
                }

                return FindPage(queryOver, pageRequest);
            };

            return HibernateTemplate.Execute(finder);
        }

        /// <summary>
        /// Ruft seitenweise Board-Vorlagen ab.
        /// </summary>
        /// <param name="pageRequest">Seiteninformation</param>
        /// <param name="user">Der Nutzer, für den die Vorlagen abgerufen werden sollen.</param>
        /// <returns>Die Board-Vorlagen</returns>
        public IPage<Board> FindBoardTemplatesForUser(IPageable pageRequest, User user) {
            Require.NotNull(pageRequest, "pageRequest");

            HibernateDelegate<IPage<Board>> finder = delegate (ISession session) {
                
                IQueryOver<Board, Board> queryOver = session.QueryOver<Board>()
                    .Where(board => board.IsTemplate)
                    .OrderBy(board => board.CreatedAt).Desc;

                return FindPage(queryOver, pageRequest);
            };

            return HibernateTemplate.Execute(finder);
        }

        /// <summary>
        /// Ruft seitenweise Boards ab, denen ein bestimmtes Team zugewiesen ist.
        /// </summary>
        /// <param name="pageRequest">Seiteninformationen</param>
        /// <param name="team">Das Team, für welches die Boards abgerufen werden sollen, denen es zugewiesen ist.</param>
        /// <returns></returns>
        public IPage<Board> FindBoardsWithTeam(IPageable pageRequest, Team team) {
            Require.NotNull(pageRequest, "pageRequest");

            HibernateDelegate<IPage<Board>> finder = delegate (ISession session) {

                Team joinTeam = null;
                QueryOver<Board, Board> teamBoards = QueryOver.Of<Board>().JoinAlias(x => x.Teams, () => joinTeam, JoinType.RightOuterJoin).Where(t => joinTeam.Id == team.Id).Select(b => b.Id);
                IQueryOver<Board, Board> queryOver = session.QueryOver<Board>().WithSubquery.WhereProperty(board => board.Id).In(teamBoards);
                
                return FindPage(queryOver, pageRequest);
            };

            return HibernateTemplate.Execute(finder);

        }
    }
}