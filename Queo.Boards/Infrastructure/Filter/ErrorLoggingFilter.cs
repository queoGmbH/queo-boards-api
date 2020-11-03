using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Filters;
using Common.Logging;

namespace Queo.Boards.Infrastructure.Filter {
    public class ErrorLoggingFilterAttribute : ActionFilterAttribute {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext) {
            ILog logger = LogManager.GetLogger("HttpErrors");

            if (actionExecutedContext.Response == null) {
                return;
            }

            if (!actionExecutedContext.Response.IsSuccessStatusCode) {
                logger.InfoFormat("Der Http-Request war nicht erfolgreich! Status-Code: {0}", actionExecutedContext.Response.StatusCode);
                logger.DebugFormat("Http-Request-Header: \r\n{0}", DictionaryToString(actionExecutedContext.Request.Headers.ToDictionary(h => h.Key, h => string.Join("; ", h.Value))));
                logger.DebugFormat("Http-Request-Eigenschaften: \r\n{0}", DictionaryToString(actionExecutedContext.Request.Properties.ToDictionary(h => h.Key, h => string.Join("; ", h.Value))));
                if (actionExecutedContext.Exception != null) {
                    logger.ErrorFormat("Während des Http-Requests [{0}] ist ein Fehler aufgetreten:", actionExecutedContext.Exception, actionExecutedContext.Request.RequestUri);
                }
                logger.InfoFormat("Http-Response: {0}", actionExecutedContext.Response.RequestMessage);
            }
        }

        private static string DictionaryToString<T>(IDictionary<string, T> dictionary) {
            if (dictionary == null) {
                return "";
            } else {
                return string.Join(Environment.NewLine, dictionary.Select(d => string.Format("{0}: {1}", d.Key, d.Value)));
            }
        }
    }
}