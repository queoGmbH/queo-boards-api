using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Spring.Data.NHibernate.Generic;

namespace Queo.Boards.Core.Persistence.Impl {
    /// <summary>
    ///     Dao für <see cref="List" />
    /// </summary>
    public class ListDao : GenericDao<List, int>, IListDao {
        public override void Delete(List entity) {
            entity.Board.Lists.Remove(entity);
            base.Delete(entity);
        }

        public IList<List> FindAllListsAndCardsByBoardId(Guid boardId) {
            Require.NotNull(boardId, nameof(boardId));

            FindHibernateDelegate<List> finder = delegate(ISession session) {
                ICriteria criteria = session.CreateCriteria<List>()
                    .CreateAlias(nameof(List.Board), "board")
                    .CreateAlias(nameof(List.Cards), "card", JoinType.LeftOuterJoin)
                    .Add(Restrictions.Eq("board.BusinessId", boardId));

                return criteria.List<List>();
            };

            IList<List> lists = HibernateTemplate.ExecuteFind(finder);
            return lists;
        }
    }
}