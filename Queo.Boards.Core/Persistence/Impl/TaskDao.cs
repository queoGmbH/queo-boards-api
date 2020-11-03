using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Core.Persistence.Impl {
    /// <summary>
    ///     Dao für <see cref="Task" />
    /// </summary>
    public class TaskDao : GenericDao<Task, int>, ITaskDao {
        /// <summary>
        ///     Löscht die übergebene Entität
        /// </summary>
        /// <param name="entity"></param>
        public override void Delete(Task entity) {
            entity.Checklist.Tasks.Remove(entity);
            base.Delete(entity);
        }
    }
}