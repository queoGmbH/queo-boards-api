using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Core.Persistence {
    /// <summary>
    /// Schnittstelle, die einen Dao beschreibt, der Methoden zur Persistierung von <see cref="Team"/>s anbietet.
    /// </summary>
    public interface ITeamDao : IGenericDao<Team, int> {

    }
}