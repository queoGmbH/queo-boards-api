using FluentNHibernate.Mapping;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence.Mappings;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence.Mappings.UserTypes;

namespace Queo.Boards.Core.Persistence.Mappings {
    /// <summary>
    ///     Mapping einer <see cref="Card" />
    /// </summary>
    public class CardMap : EntityMap<Card> {
        /// <summary>
        /// </summary>
        protected CardMap() {
            Map(x => x.Title).Not.Nullable();
            Map(x => x.Description).CustomSqlType("NVARCHAR(MAX)");
            Map(x => x.Due).Nullable().CustomType<SaveAndLoadAsUtcDateTimeType>();

            Map(x => x.DueExpirationNotificationCreated).Not.Nullable();
            Map(x => x.DueExpirationNotificationCreatedAt).Nullable().CustomType<SaveAndLoadAsUtcDateTimeType>();

            Map(x => x.IsArchived).Not.Nullable().Default("0");
            Map(x => x.ArchivedAt).Nullable().CustomType<SaveAndLoadAsUtcDateTimeType>();

            Map(x => x.CreatedAt).Not.Nullable().CustomType<SaveAndLoadAsUtcDateTimeType>();
            References(x => x.CreatedBy)
                    .Not.Nullable()
                    .ForeignKey("FK_Card_Creator");

            References(x => x.List)
                    .Not.Nullable()
                    .Not.Insert()
                    .Not.Update()
                    .ForeignKey("FK_Card_List");

            HasManyToMany(x => x.Labels).Table("tblLabelToCard").ForeignKeyConstraintNames("FK_LabelToCard_Card", "FK_LabelToCard_Label");

            HasMany(x => x.Comments).Cascade.Delete().Inverse();
            HasMany(x => x.Checklists).Cascade.Delete().Inverse();
            HasMany(x => x.Documents).Inverse();
            HasMany(x => x.CardNotifications).Cascade.Delete().Inverse();

            HasManyToMany(x => x.AssignedUsers)
                    .Table("tblCardAssignedUsers")
                    .ForeignKeyConstraintNames("FK_CARD_WITH_ASSIGNED_USERS", "FK_USER_ASSIGNED_TO_CARD")
                    .ParentKeyColumn("Card_Id")
                    .ChildKeyColumn("User_Id")
                    .Access.CamelCaseField(Prefix.Underscore);
        }
    }
}