using FluentNHibernate.Mapping;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence.Mappings;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence.Mappings.UserTypes;

namespace Queo.Boards.Core.Persistence.Mappings {
    /// <summary>
    ///     Mapping für <see cref="Queo.Boards.Core.Domain.List" />
    /// </summary>
    public class ListMap : EntityMap<List> {
        /// <summary>
        /// </summary>
        protected ListMap() {
            Map(x => x.Title).Not.Nullable();

            Map(x => x.IsArchived).Not.Nullable().Default("0");
            Map(x => x.ArchivedAt).Nullable().CustomType<SaveAndLoadAsUtcDateTimeType>();

            References(x => x.Board)
                    .Not.Nullable()
                    .Not.Insert()
                    .Not.Update()
                    .ForeignKey("FK_List_Board");

            HasMany(x => x.Cards)
                .Access.CamelCaseField(Prefix.Underscore)
                .AsList(index => index.Column("PositionInList"))
                .Cascade.All()
                .Not.KeyNullable();
        }
    }
}