using Queo.Boards.Core.Domain.Activities;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Core.Persistence {
    /// <summary>
    ///     Schnittstelle für Daos für <see cref="BoardActivity" />
    /// </summary>
    public interface IBoardActivityDao : IGenericDao<BoardActivity, int> {
    }
}