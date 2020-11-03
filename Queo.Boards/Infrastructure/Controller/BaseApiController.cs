using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using FluentValidation.Results;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Infrastructure.Controller {
    /// <summary>
    ///     Api Controller mit zusätzlichen Aufruf-Rückgabewerten
    /// </summary>
    public abstract class BaseApiController : ApiController {
        /// <summary>
        ///     422 - Unprocessable Entity as in https://tools.ietf.org/html/rfc4918#section-11.2
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected ResponseMessageResult UnprocessableEntity(string message) {
            return new ResponseMessageResult(new HttpResponseMessage((HttpStatusCode)422) {Content = new StringContent(message)});
        }

        /// <summary>
        ///     422 - Unprocessable Entity as in https://tools.ietf.org/html/rfc4918#section-11.2
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        protected ResponseMessageResult UnprocessableEntity(IList<ValidationFailure> errors) {
            string aggregatedError = string.Join(" | ", errors.Select(x => x.ErrorMessage));
            return new ResponseMessageResult(new HttpResponseMessage((HttpStatusCode)422) { Content = new StringContent(aggregatedError) });
        }

        protected ResponseMessageResult PolicyNotFulfilled(string message) {
            return new ResponseMessageResult(new HttpResponseMessage((HttpStatusCode)420) {Content = new StringContent(message)});
        }


        protected IHttpActionResult Conflict(string message) {
            return new ResponseMessageResult(new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(message) });
        }

        protected IHttpActionResult Forbidden(string message) {
            return new ResponseMessageResult(new HttpResponseMessage(HttpStatusCode.Forbidden) { Content = new StringContent(message) });
        }

        protected IHttpActionResult DocumentStream(Document document, Stream documentStream, string overrideContentType = null) {
            HttpResponseMessage response = this.Request.CreateResponse();
            response.Content = new StreamContent(documentStream);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") {
                FileName = document.Name
            };
            if (string.IsNullOrWhiteSpace(overrideContentType)) {
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(document.Name));
            } else {
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(overrideContentType);
            }
            response.Content.Headers.ContentLength = documentStream.Length;
            
            return new ResponseMessageResult(response);
        }
    }
}