using FluentNHibernate.Mapping;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Activities;

namespace Queo.Boards.Core.Persistence.Mappings.Activities {
    /// <summary>
    ///     Mapping für <see cref="BoardActivity" />
    /// </summary>
    public class BoardActivityMap : SubclassMap<BoardActivity> {
        /// <summary>
        /// </summary>
        public BoardActivityMap() {
            DiscriminatorValue(ActivityType.Board);
            References(x => x.Board).Not.Nullable().ForeignKey("FK_Activity_Board");
        }
    }
}