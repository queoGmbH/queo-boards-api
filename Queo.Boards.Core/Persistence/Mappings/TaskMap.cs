using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence.Mappings;

namespace Queo.Boards.Core.Persistence.Mappings {
    /// <summary>
    ///     Mapping für <see cref="Task" />
    /// </summary>
    public class TaskMap : EntityMap<Task> {
        
        
        /// <summary>
        /// </summary>
        protected TaskMap() {
            Map(x => x.Title);
            Map(x => x.IsDone).Default("0").Not.Nullable();

            References(x => x.Checklist).Not.Nullable().ForeignKey("FK_Task_Checklist");
        }
    }
}