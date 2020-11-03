using FluentNHibernate.Mapping;
using Queo.Boards.Core.Infrastructure.NHibernate.Domain;

namespace Queo.Boards.Core.Infrastructure.NHibernate.Persistence.Mappings {
    public class EntityMap<T>:ClassMap<T> where T:Entity {
         protected EntityMap() {
            LazyLoad();
            Id(x => x.Id);
            Map(x => x.BusinessId).Unique().Not.Nullable();
        }
    }
}