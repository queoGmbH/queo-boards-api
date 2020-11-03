using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Core.Persistence {
    /// <summary>
    ///     Schnittstelle für Daos für <see cref="Task" />
    /// </summary>
    public interface ITaskDao : IGenericDao<Task, int> {
    }
}