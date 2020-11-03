using FluentNHibernate.Mapping;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence.Mappings;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence.Mappings.UserTypes;

namespace Queo.Boards.Core.Persistence.Mappings {
    /// <summary>
    ///     Definiert das NHibernate-Mapping für ein Team
    /// </summary>
    public class TeamMap : EntityMap<Team> {
        protected TeamMap() {
            Map(team => team.Name)
                    .Not.Nullable()
                    .Length(75);

            Map(team => team.Description).Nullable().CustomSqlType("nvarchar(max)");

            HasManyToMany(x => x.Members)
                    .Table("tblTeamMember")
                    .ForeignKeyConstraintNames("FK_TeamMember_Team", "FK_TeamMember_User")
                    .ParentKeyColumn("Team_Id")
                    .ChildKeyColumn("User_Id")
                    .Access.CamelCaseField(Prefix.Underscore)
                    /*Damit ein Primary Key erstellt wird, der gleichzeitig ein Unique Index ist*/
                    .AsSet();

            Map(x => x.CreatedAt).CustomType<SaveAndLoadAsUtcDateTimeType>();
            References(x => x.CreatedBy).NotFound.Ignore().Not.Nullable().ForeignKey("FK_Team_Creator_User");
        }
    }
}