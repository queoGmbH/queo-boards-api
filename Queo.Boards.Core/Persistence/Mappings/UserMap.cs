using FluentNHibernate.Mapping;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence.Mappings;

namespace Queo.Boards.Core.Persistence.Mappings {
    public class UserMap : EntityMap<User> {
        protected UserMap() {

            Map(x => x.UserName).Not.Nullable().Length(100).Unique();
            Map(x=>x.PasswordHash).Length(255);

            Map(x => x.UserCategory).Not.Nullable().Default("'Local'");

            Map(x => x.Firstname).Length(200);
            Map(x => x.Lastname).Length(200);

            Map(x => x.Email).Length(200);
            Map(x => x.Phone).Length(200);
            Map(x => x.Company).Length(200);
            Map(x => x.Department).Length(200);

            Map(x => x.IsEnabled).Not.Nullable();

            Map(x => x.PasswordResetRequestId).Nullable();
            Map(x => x.PasswordResetRequestValidTo).Nullable();

            HasMany(user => user.Roles)
                    .KeyColumn("User_Id")
                    .Element("Role")
                    .Table("tblUserRoles")
                    .Cascade.AllDeleteOrphan()
                    .ForeignKeyConstraintName("FK_ROLE_TO_USER")
                    .Access.CamelCaseField(Prefix.Underscore);
        }
    }
}