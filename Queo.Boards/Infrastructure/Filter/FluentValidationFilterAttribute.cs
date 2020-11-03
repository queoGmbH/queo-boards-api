using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Common.Logging;
using FluentValidation;
using FluentValidation.Attributes;
using FluentValidation.Results;
using Spring.Context.Support;

namespace Queo.Boards.Infrastructure.Filter {
    /// <summary>
    ///     Filter der für jeden Request prüft, ob Validierungen der Parameter vorgenommen werden müssen.
    ///     Dazu werden die Parameter der aufgerufenen Controller-Methode, auf das <see cref="ValidatorAttribute" /> geprüft.
    ///     Ist ein Parameter mit diesem Attribute dekoriert, wird eine Validierung des Parameters durchgeführt. Schlägt die
    ///     Validierung fehl, wird ein ModelError gesetzt und der <see cref="HttpActionContext.ModelState" /> auf Invalid
    ///     gesetzt.
    /// </summary>
    public class FluentValidationFilterAttribute : ActionFilterAttribute {
        /// <summary>Tritt vor dem Aufrufen der Aktionsmethode auf.</summary>
        /// <param name="actionContext">Der Aktionskontext.</param>
        public override void OnActionExecuting(HttpActionContext actionContext) {
            ILog logger = LogManager.GetLogger("Validation");

            logger.DebugFormat("Überprüfung, ob eine Validierung erfolgen soll.");
            if (actionContext.ActionDescriptor is ReflectedHttpActionDescriptor) {
                ParameterInfo[] parameterInfos = (actionContext.ActionDescriptor as ReflectedHttpActionDescriptor).MethodInfo.GetParameters();

                foreach (ParameterInfo parameterInfo in parameterInfos) {
                    IEnumerable<ValidatorAttribute> validatorAttributes = parameterInfo.GetCustomAttributes().OfType<ValidatorAttribute>();
                    foreach (ValidatorAttribute validatorAttribute in validatorAttributes) {
                        Type validatorType = validatorAttribute.ValidatorType;
                        IDictionary<string, object> possibleValidators = ContextRegistry.GetContext().GetObjectsOfType(validatorType);
                        if (!possibleValidators.Any()) {
                            logger.Error($"can't find validator from type {validatorType}. Not configured in spring.validators.config?");
                            throw new InvalidOperationException("Wrong configuration. Can't find expected validator.");
                        }
                        IValidator validator = possibleValidators.Single().Value as IValidator;
                        logger.DebugFormat("Der Parameter {0} soll mit dem {1} validiert werden.", parameterInfo.Name, validatorType.FullName);

                        if (validator == null) {
                            logger.ErrorFormat("Es konnte keine Instanz des Validator-Typs {0} erstellt werden, da dieser entweder nicht im Spring-Kontext definiert ist oder die Schnittstelle IValidiator nicht implementiert.", validatorType.FullName);
                            break;
                        }

                        ValidationResult validationResult = validator.Validate(actionContext.ActionArguments[parameterInfo.Name]);
                        if (!validationResult.IsValid) {
                            foreach (ValidationFailure validationFailure in validationResult.Errors) {
                                if (!string.IsNullOrWhiteSpace(validationFailure.PropertyName)) {
                                    string errorPropertyName = parameterInfo.Name + "." + validationFailure.PropertyName;
                                    actionContext.ModelState.AddModelError(errorPropertyName, validationFailure.ErrorMessage);
                                    logger.InfoFormat("Der Validierung der Eigenschaft {0}, des Parameters {1} mit dem {2} war nicht erfolgreich: {3}", validationFailure.PropertyName, parameterInfo.Name, validatorType.FullName, validationFailure.ErrorMessage);
                                } else {
                                    actionContext.ModelState.AddModelError(parameterInfo.Name, validationFailure.ErrorMessage);
                                    logger.InfoFormat("Der Validierung des Parameters {0} mit dem {1} war nicht erfolgreich: {2}", parameterInfo.Name, validatorType.FullName, validationFailure.ErrorMessage);
                                }
                            }
                            actionContext.Response = actionContext.Request.CreateErrorResponse((HttpStatusCode)422, actionContext.ModelState);
                        } else {
                            logger.DebugFormat("Die Validierung des Parameters {0} mit dem {1} war erfolgreich.", parameterInfo.Name, validatorType.FullName);
                        }
                    }
                }
            } else {
                logger.InfoFormat("Es konnte keine Validierung durchgeführt werden, da der ActionDescriptor des Kontext nicht vom Typ {0} ist.", typeof(ReflectedHttpActionDescriptor));
            }

            base.OnActionExecuting(actionContext);
            
        }
    }
}