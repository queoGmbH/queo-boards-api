using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence.Mappings;

namespace Queo.Boards.Core.Persistence.Mappings {
    /// <summary>
    /// Mapping für <see cref="Document"/>
    /// </summary>
    public class DocumentMap : EntityMap<Document> {
        /// <summary>
        /// 
        /// </summary>
        protected DocumentMap() {
            Map(x => x.Name).Not.Nullable();
            References(x => x.Card).Not.Nullable().ForeignKey("FK_Document_Card");
        }
    }
}