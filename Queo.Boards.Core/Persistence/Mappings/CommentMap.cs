using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence.Mappings;

namespace Queo.Boards.Core.Persistence.Mappings {
    /// <summary>
    ///     Mapping für <see cref="Comment" />
    /// </summary>
    public class CommentMap : EntityMap<Comment> {
        /// <summary>
        /// </summary>
        protected CommentMap() {
            Map(x => x.Text).CustomSqlType("NVARCHAR(MAX)");
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.IsDeleted).Not.Nullable();

            HasMany(x => x.CommentNotifications).Cascade.Delete().Inverse();

            References(x => x.Creator).Not.Nullable().ForeignKey("FK_Comment_User_Creator");
            References(x => x.Card).Not.Nullable().ForeignKey("FK_Comment_Card");
        }
    }
}