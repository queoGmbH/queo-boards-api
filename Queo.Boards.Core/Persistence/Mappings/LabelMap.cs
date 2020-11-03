using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence.Mappings;

namespace Queo.Boards.Core.Persistence.Mappings {
    /// <summary>
    ///     Map für <see cref="Label" />
    /// </summary>
    public class LabelMap : EntityMap<Label> {
        /// <summary>
        /// </summary>
        protected LabelMap() {
            Map(x => x.Name).Not.Nullable();
            Map(x => x.Color).Not.Nullable();

            References(x => x.Board)
                .Not.Nullable()
                .Not.Insert()
                .Not.Update()
                .ForeignKey("FK_Label_Board");
        }
    }
}