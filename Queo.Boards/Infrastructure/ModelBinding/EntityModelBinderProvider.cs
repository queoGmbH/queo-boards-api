using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Queo.Boards.Core.Infrastructure.NHibernate.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Infrastructure.Utils;
using IModelBinder = System.Web.Http.ModelBinding.IModelBinder;

namespace Queo.Boards.Infrastructure.ModelBinding {
    public class EntityModelBinderProvider : ModelBinderProvider {
        /// <summary>Sucht einen Binder für den angegebenen Typ.</summary>
        /// <returns>
        ///     Ein Binder, der versuchen kann, diesen Typ zu binden. Oder "null", wenn der Binder statisch weiß, dass er
        ///     diesen Typ niemals binden kann.
        /// </returns>
        /// <param name="configuration">Ein Konfigurationsobjekt.</param>
        /// <param name="modelType">Der Typ des Modells, an das gebunden werden soll.</param>
        public override IModelBinder GetBinder(HttpConfiguration configuration, Type modelType) {
            IModelBinder modelBinder = null;
            if (typeof(Entity).IsAssignableFrom(modelType)){
                Type modelBinderType = typeof(EntityModelBinder<>).MakeGenericType(modelType);
                Type daoType = typeof(IGenericDao<,>).MakeGenericType(modelType, typeof(int));
                modelBinder = (IModelBinder)Activator.CreateInstance(modelBinderType, daoType);
            } else if (TypeHelper.IsDomainEntityTypeList(modelType)) {
                Type entityType = modelType.GetGenericArguments().First();
                Type modelBinderType = typeof(EntityListModelBinder<>).MakeGenericType(entityType);
                Type daoType = typeof(IGenericDao<,>).MakeGenericType(new[] { entityType, typeof(int) });
                modelBinder = (IModelBinder)Activator.CreateInstance(modelBinderType, daoType);
            }

            return modelBinder;
        }
    }
}