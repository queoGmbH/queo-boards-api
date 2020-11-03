using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Core.Persistence.Impl {

    /// <summary>
    /// Dao der Methoden zur Persistierung von <see cref="Team"/>s anbietet.
    /// </summary>
    public class TeamDao : GenericDao<Team, int>, ITeamDao {
        
    }
}