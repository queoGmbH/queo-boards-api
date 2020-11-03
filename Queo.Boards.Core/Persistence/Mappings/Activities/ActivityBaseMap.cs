using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Activities;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence.Mappings;

namespace Queo.Boards.Core.Persistence.Mappings.Activities {
    /// <summary>
    ///     Mapping für die Basisaktivität.
    ///     Die Aktivitäten werden anhand des <see cref="ActivityType" /> unterschieden
    /// </summary>
    public class ActivityBaseMap : EntityMap<ActivityBase> {
        /// <summary>
        /// </summary>
        protected ActivityBaseMap() {
            DiscriminateSubClassesOnColumn("ActivityType");
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.Text).Not.Nullable();
            References(x => x.Creator).Not.Nullable().ForeignKey("FK_Activity_Creator_User");
        }
    }
}