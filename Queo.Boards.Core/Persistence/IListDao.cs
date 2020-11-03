using System;
using System.Collections.Generic;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Core.Persistence {
    /// <summary>
    ///     Schnittstelle für Daos für <see cref="List" />
    /// </summary>
    public interface IListDao : IGenericDao<List, int> {
        IList<List> FindAllListsAndCardsByBoardId(Guid boardId);
    }
}