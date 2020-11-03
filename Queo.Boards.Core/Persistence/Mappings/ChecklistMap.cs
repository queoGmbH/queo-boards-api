using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence.Mappings;

namespace Queo.Boards.Core.Persistence.Mappings {
    /// <summary>
    ///     Mapping für <see cref="Checklist" />
    /// </summary>
    public class ChecklistMap : EntityMap<Checklist> {
        /// <summary>
        /// </summary>
        protected ChecklistMap() {
            Map(x => x.Title).Not.Nullable();

            References(x => x.Card).Not.Nullable().ForeignKey("FK_Checklist_Card");

            HasMany(x => x.Tasks).Inverse().Cascade.Delete();
        }
    }
}