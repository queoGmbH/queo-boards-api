using Queo.Boards.Core.Domain.Activities;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Core.Persistence.Impl {
    /// <summary>
    ///     Dao für <see cref="ActivityBase" />
    /// </summary>
    public class ActivityBaseDao : GenericDao<ActivityBase, int>, IActivityBaseDao {
    }
}