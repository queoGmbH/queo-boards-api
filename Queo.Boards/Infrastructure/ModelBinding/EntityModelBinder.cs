using System;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Common.Logging;
using Queo.Boards.Core.Infrastructure.NHibernate.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Persistence;
using IModelBinder = System.Web.Http.ModelBinding.IModelBinder;
using ModelBindingContext = System.Web.Http.ModelBinding.ModelBindingContext;
using ValueProviderResult = System.Web.Http.ValueProviders.ValueProviderResult;

namespace Queo.Boards.Infrastructure.ModelBinding {
    public class EntityModelBinder<TEntity> : IModelBinder
            where TEntity : Entity {
        private readonly Type _daoType;

        private readonly ILog _logger = LogManager.GetLogger(typeof(EntityModelBinder<>));
        private IGenericDao<TEntity, int> _dao;

        public EntityModelBinder(Type daoType) {
            _daoType = daoType;
        }

        /// <summary>
        ///     Bindet das Modell mithilfe des angegebenen Controllerkontexts und Bindungskontexts an einen Wert.
        /// </summary>
        /// <returns>
        ///     Der gebundene Wert.
        /// </returns>
        /// <param name="controllerContext">Der Controllerkontext.</param>
        /// <param name="bindingContext">Der Bindungskontext.</param>
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext) {
            object dao = DependencyResolver.Current.GetService(_daoType);
            _dao = (IGenericDao<TEntity, int>)dao;

            string modelName = bindingContext.ModelName;

            if (IsSpecialNamedParameter(modelName)) {
                return BindSpecialValue(modelName, actionContext, bindingContext);
            } else {
                ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
                if (valueProviderResult != null) {
                    try {
                        bindingContext.Model = GetSingleValue(valueProviderResult.AttemptedValue);
                    } catch (Exception ex) {
                        _logger.ErrorFormat("Beim Binden eines Objektes vom Typ [{0}] mit der Id [{1}] ist ein Fehler aufgetreten.",
                            ex,
                            bindingContext.ModelType,
                            valueProviderResult.AttemptedValue);
                        bindingContext.Model = null;
                        actionContext.ModelState.AddModelError("Fehler beim Binden " + valueProviderResult.AttemptedValue, ex);
                        return true;
                    }
                }
            }

            return true;
        }

        private bool BindSpecialValue(string modelName, HttpActionContext actionContext, ModelBindingContext bindingContext) {
            if (modelName.Equals("currentUser")) {
                //string userId = controllerContext.HttpContext.User.Identity.GetUserId();
                // Die Id = 0 kann es bei einem Nutzer nicht geben. Wir brauchen es nicht erst versuchen.
                //if (userId == 0) { return null;}
                //_logger.DebugFormat("Versuche den current User mit der Id {0} zu laden. ", userId);
                try {
                    string name = HttpContext.Current.User.Identity.Name;
                    string identityName = actionContext.RequestContext.Principal.Identity.Name;
                    bindingContext.Model = ((IUserDao)_dao).FindByUsername(identityName);
                } catch (Exception ex) {
                    _logger.Error(ex);
                    return false;
                }
                return true;
            }
            return false;
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

        private bool IsSpecialNamedParameter(string modelName) {
            return modelName.Equals("currentUser");
        }
    }
}