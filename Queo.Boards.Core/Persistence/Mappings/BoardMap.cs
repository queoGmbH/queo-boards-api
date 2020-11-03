using FluentNHibernate.Mapping;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence.Mappings;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence.Mappings.UserTypes;

namespace Queo.Boards.Core.Persistence.Mappings {
    public class BoardMap : EntityMap<Board> {
        protected BoardMap() {
            Map(x => x.Accessibility).Not.Nullable();
            
            Map(x => x.Title);
            Map(x => x.ColorScheme);
            Map(x => x.IsArchived).Not.Nullable().Default("0");
            Map(x => x.IsTemplate).Not.Nullable().Default("0");
            Map(x => x.ArchivedAt).Nullable().CustomType<SaveAndLoadAsUtcDateTimeType>();

            Map(x => x.CreatedAt).CustomType<SaveAndLoadAsUtcDateTimeType>();
            References(x => x.CreatedBy).Column("Creator_Id").NotFound.Ignore().Not.Nullable().ForeignKey("FK_Board_Creator_User");

            HasManyToMany(x => x.Owners)
                    .Table("tblBoardOwner")
                    .ForeignKeyConstraintNames("FK_BoardOwner_Board", "FK_BoardOwner_User")
                    .ParentKeyColumn("Board_Id")
                    .ChildKeyColumn("User_Id")
                    .Access.CamelCaseField(Prefix.Underscore)
                    /*Damit ein Primary Key erstellt wird, der gleichzeitig ein Unique Index ist*/
                    .AsSet();

            HasManyToMany(x => x.Members)
                    .Table("tblBoardMember")
                    .ForeignKeyConstraintNames("FK_BoardMember_Board", "FK_BoardMember_User")
                    .ParentKeyColumn("Board_Id")
                    .ChildKeyColumn("User_Id")
                    .Access.CamelCaseField(Prefix.Underscore)
                    /*Damit ein Primary Key erstellt wird, der gleichzeitig ein Unique Index ist*/
                    .AsSet();

            HasManyToMany(x => x.Teams)
                    .Table("tblBoardTeams")
                    .ForeignKeyConstraintNames("FK_BoardTeams_Board", "FK_BoardTeams_Team")
                    .ParentKeyColumn("Board_Id")
                    .ChildKeyColumn("Team_Id")
                    .Access.CamelCaseField(Prefix.Underscore)
                    /*Damit ein Primary Key erstellt wird, der gleichzeitig ein Unique Index ist*/
                    .AsSet();

            HasMany(x => x.Lists)
                    .Access.CamelCaseField(Prefix.Underscore)
                    .AsList(index => index.Column("PositionOnBoard"))
                    .Cascade
                    .All()
                    .Not.KeyNullable();

            HasMany(x => x.Labels)
                    .Access.CamelCaseField(Prefix.Underscore)
                    .ForeignKeyConstraintName("FK_Label_Board")
                    .Cascade.All()
                    .Not.KeyNullable();
            
        }
    }
}