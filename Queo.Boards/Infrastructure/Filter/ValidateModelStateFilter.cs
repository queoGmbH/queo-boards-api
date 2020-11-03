using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Common.Logging;

namespace Queo.Boards.Infrastructure.Filter {
    /// <summary>
    ///     Filter, der bei einem Request mit invalidem <see cref="HttpActionContext.ModelState">ModelState</see> die Antwort
    ///     auf den Request manipuliert.
    ///     Dabei wird der HttpStatusCode auf 422 (UnprocessableEntity) gesetzt und ein Array mit den Validierungs-Fehlern an
    ///     die Antwort gehängt.
    /// 
    /// Eine Validierung im Controller braucht nicht mehr zu erfolgen.
    /// </summary>
    public class ValidateModelStateFilter : ActionFilterAttribute {
        private readonly ILog _logger = LogManager.GetLogger("Validation");

        /// <summary>Tritt vor dem Aufrufen der Aktionsmethode auf.</summary>
        /// <param name="actionContext">Der Aktionskontext.</param>
        public override void OnActionExecuting(HttpActionContext actionContext) {
            if (!actionContext.ModelState.IsValid) {
                _logger.InfoFormat("Der Request hatte einen ungültigen ModelState. Es wird eine Antwort mit dem StatusCode 422 und den enthaltenen Fehlern geliefert: {0}", actionContext.ModelState);
                actionContext.Response = actionContext.Request.CreateErrorResponse((HttpStatusCode)422, actionContext.ModelState);
            } else {
                _logger.DebugFormat("Der Request hat einen validen ModelState.");
                base.OnActionExecuting(actionContext);
            }
        }

        /// <summary>Tritt nach dem Aufrufen der Aktionsmethode auf.</summary>
        /// <param name="actionExecutedContext">Der Kontext nach der Ausführung der Aktion.</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext) {

            if (actionExecutedContext.Response == null) {
                return;
            }

            if (!actionExecutedContext.ActionContext.ModelState.IsValid && actionExecutedContext.Response.StatusCode != (HttpStatusCode)422) {
                _logger.InfoFormat("Nach dem Ausführen des Request war der ModelState ungültig. Es wird eine Antwort mit dem StatusCode 422 und den enthaltenen Fehlern geliefert: {0}", actionExecutedContext.ActionContext.ModelState);
                actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse((HttpStatusCode)422, actionExecutedContext.ActionContext.ModelState);
            } else {
                _logger.DebugFormat("Nach dem Ausführen des Requests war der ModelState valide.");
                base.OnActionExecuted(actionExecutedContext);
            }


            
        }
    }
}