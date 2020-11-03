using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Common.Logging;
using NSwag.Annotations;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Models.Builders;
using Queo.Boards.Core.Services;
using Queo.Boards.Infrastructure.Controller;

namespace Queo.Boards.Controllers {
    /// <summary>
    ///     Controller für <see cref="Document" />
    /// </summary>
    [RoutePrefix("api")]
    public class AttachmentController : AuthorizationRequiredApiController {

        private static readonly ILog _logger = LogManager.GetLogger(typeof(AttachmentController));
        private readonly IDocumentService _documentService;

        /// <summary>
        /// </summary>
        /// <param name="documentService"></param>
        public AttachmentController(IDocumentService documentService) {
            _documentService = documentService;
        }

        /// <summary>
        ///     Löscht ein Dokument/Anhang
        /// </summary>
        /// <param name="attachment">Id des Dokuments als Guid</param>
        /// <param name="currentUser">Der Nutzer der die Datei löscht.</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("attachments/{attachment:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(AttachmentModel))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void),
             Description = "Wenn das Attachment nicht gefunden wurde oder die Karte des Attachments archiviert ist.")]
        public IHttpActionResult DeleteDocument([ModelBinder] Document attachment, [ModelBinder, SwaggerIgnore]User currentUser) {
            Require.NotNull(currentUser, "currentUser");

            if (attachment == null || attachment.Card.IsArchived) {
                return NotFound();
            }
            Document deletedDocument = _documentService.DeleteDocument(attachment);
            return Ok(DocumentModelBuilder.Build(deletedDocument, currentUser));
        }

        /// <summary>
        ///     Liefert den Download des Dokuments
        /// </summary>
        /// <param name="attachment">Id des Anhangs als Guid</param>
        /// <param name="currentUser"></param>
        /// <returns>Dateidownload</returns>
        [HttpGet]
        [Route("attachments/{attachment:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void),
             Description = "Wenn das Attachment nicht gefunden wurde oder die Karte des Attachments archiviert ist.")]
        [AllowAnonymous]
        public HttpResponseMessage DownloadAttachment([ModelBinder] Document attachment, [SwaggerIgnore] [ModelBinder] User currentUser) {
            if (attachment == null || attachment.Card.IsArchived) {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(_documentService.GetDocumentStream(attachment));
            result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(attachment.Name));
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") {
                FileName = attachment.Name
            };
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="attachment">Id des Anhangs als Guid</param>
        /// <param name="currentUser"></param>
        /// <param name="width">Breite</param>
        /// <param name="height">Höhe</param>
        /// <returns>Dateidownload</returns>
        [HttpGet]
        [Route("attachments/{attachment:Guid}/thumbnail")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void),
             Description = "Wenn das Attachment nicht gefunden wurde oder die Karte des Attachments archiviert ist.")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetAttachmentThumbnailAsync([ModelBinder] Document attachment,
            [SwaggerIgnore] [ModelBinder] User currentUser, int width, int height) {
            if (attachment == null || attachment.Card.IsArchived) {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            try {
                FileStream thumbnailStream = await _documentService.GetThumbnailForDocumentAsync(attachment, width, height);
                string thumbnailFileName = attachment.Name + ".jpg";
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(thumbnailStream);
                result.Content.Headers.ContentType =
                        new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(thumbnailFileName));
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") {
                    FileName = thumbnailFileName
                };
                return result;
            } catch (Exception ex) {
                _logger.Error("Error on thumbnail creation", ex);
                if (ex.InnerException != null) {
                    _logger.Error("Error on thumbnail creation", ex.InnerException);
                }
            }
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }
}