using System;
using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;
using Common.Logging;
using Queo.Boards.Core.Infrastructure.NHibernate.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Infrastructure.ModelBinding {
    /// <summary>
    ///     Klasse implementiert das Binden von Listen von Entities.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class EntityListModelBinder<TEntity> : IModelBinder where TEntity : Entity {
        private readonly IGenericDao<TEntity, int> _dao;
        private readonly ILog _logger = LogManager.GetLogger(typeof(EntityModelBinder<>));

        public EntityListModelBinder(IGenericDao<TEntity, int> dao) {
            _dao = dao;
        }

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext) {
            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            /*Es handelt sich um eine Liste die gebunden werden muss*/
            List<TEntity> domainEntities = new List<TEntity>();
            if (valueProviderResult == null) {
                bindingContext.Model = domainEntities;
                return true;
            }
            IList<object> listOfDomainEntityKeys = valueProviderResult.ConvertTo(typeof(IList<object>)) as IList<object>;
            if (listOfDomainEntityKeys != null) {
                foreach (object domainEntityId in listOfDomainEntityKeys) {
                    TEntity domainEntityValue;
                    try {
                        domainEntityValue = GetSingleValue(domainEntityId);
                        if (!domainEntities.Contains(domainEntityValue)) {
                            /*TODO: Ist null ein valider Wert?*/
                            domainEntities.Add(domainEntityValue);
                        }
                    } catch (Exception ex) {
                        _logger.ErrorFormat("Beim Binden eines Objektes für eine Liste vom Typ [{0}] mit der Id [{1}] ist ein Fehler aufgetreten.", ex, bindingContext.ModelType, valueProviderResult.AttemptedValue);

                        bindingContext.Model = null;
                        actionContext.ModelState.AddModelError("Fehler beim Binden " + valueProviderResult.AttemptedValue, ex);
                        return true;
                    }
                }
            }

            bindingContext.Model = domainEntities;
            return true;
        }

        private TEntity GetSingleValue(object modelValue) {
            if (modelValue == null) {
                return null;
            }
            Guid businessId;
            int id;
            TEntity entity = null;
            if (Guid.TryParse(modelValue.ToString(), out businessId)) {
                /*Domain-Entity wird anhand der BusinessId gebunden*/
                entity = _dao.GetByBusinessId(businessId);
            } else if (int.TryParse(modelValue.ToString(), out id)) {
                /*Domain-Entity wird anhand der Id gebunden*/
                entity = _dao.GetByPrimaryKey(id);
            }
            return entity;
        }
    }
}