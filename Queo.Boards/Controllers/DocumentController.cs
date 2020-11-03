using System;
using System.Net;
using System.Web.Http;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Models.Builders;
using Queo.Boards.Core.Services;
using Queo.Boards.Infrastructure.Controller;

namespace Queo.Boards.Controllers {


    [RoutePrefix("api/documents")]
    public class DocumentController : AuthorizationRequiredApiController {

        private IDocumentService _documentService;

        public DocumentController(IDocumentService documentService) {
            _documentService = documentService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("download")]
        public IHttpActionResult Download(string token) {
            Document document;
            try {
                AttachmentDownload download = AttachmentDownloadTokenizer.ParseFromToken(token);
                if (download.ExpirationDate < DateTime.Now) {
                    return StatusCode(HttpStatusCode.Gone);
                }
                document = _documentService.GetByBusinessId(download.DocumentId);
            } catch (Exception e) {
                return NotFound();
            }

            try {
                return DocumentStream(document, _documentService.GetDocumentStream(document));
            } catch (Exception e) {
                return InternalServerError();
            }
        }

        

        [HttpGet]
        [AllowAnonymous]
        [Route("thumbnail")]
        public IHttpActionResult Thumbnail(string token, int width = 320, int height = 320) {
            Document document;

            try {
                AttachmentDownload download = AttachmentDownloadTokenizer.ParseFromToken(token);
                if (download.ExpirationDate < DateTime.Now) {
                    return StatusCode(HttpStatusCode.Gone);
                }
                document = _documentService.GetByBusinessId(download.DocumentId);
            } catch (Exception e) {
                return NotFound();
            }
            
            
            try {
                return DocumentStream(document, _documentService.GetThumbnailForDocumentAsync(document, width, height).Result, "image/png");
            } catch (Exception e) {
                return InternalServerError(e);
            }
        }
    }
}