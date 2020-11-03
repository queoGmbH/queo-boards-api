using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Core.Persistence.Impl {
    /// <summary>
    /// Dao für <see cref="Label"/>
    /// </summary>
    public class LabelDao : GenericDao<Label, int>, ILabelDao {
        
    }
}